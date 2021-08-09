using System;
using System.Collections.Generic;
using System.Text;

namespace RSocket
{
    public interface ISerializable<T>
    {
        public List<byte> Serialize();
        public T Deserialize(byte[] bytes);
        List<byte> SerializeLengthPrefixed();
    }

    public static class RSocketFrame
    {
        private const int FrameTypeOffset = 10;
        private const int FlagsMask = 0x3ff;

        public abstract class Frame
        {
            public RSocketFrameType Type;
            public int StreamId;
            public ushort Flags;

            protected void WriteStreamId(List<byte> bytes)
            {
                byte[] streamAsBytes = BitConverter.GetBytes(StreamId);

                WriteIntBigEndian(bytes, streamAsBytes);
            }

            protected void WriteTypeAndFlags(List<byte> bytes)
            {
                int type = (int) Type << FrameTypeOffset;
                int flags = Flags & FlagsMask;
                ushort typeAndFlags = (ushort) (type | flags);
                byte[] typeAndFlagsBytes = BitConverter.GetBytes(typeAndFlags);

                WriteIntBigEndian(bytes, typeAndFlagsBytes);
            }

            protected static void WriteIntBigEndian(List<byte> target, byte[] bytes)
            {
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(bytes);
                }

                target.AddRange(bytes);
            }
        }

        public class SetupFrame : Frame, ISerializable<SetupFrame>
        {
            public string DataMimeType;
            public string MetadataMimeType;
            public List<byte> Data;
            public List<byte> Metadata;
            public int KeepAlive;
            public int LifeTime;
            public ushort MajorVersion;
            public ushort MinorVersion;

            public List<byte> Serialize()
            {
                List<byte> bytes = new List<byte>();

                WriteCommonHeader(bytes);
                WriteHeader(bytes);
                WritePayload(bytes);

                return bytes;
            }

            private void WritePayload(List<byte> bytes)
            {
                // Check if Metadata flag is set
                if ((Flags & (int) RSocketFlagType.METADATA) == (int) RSocketFlagType.METADATA)
                {
                    // Write metadata with length prefix if we have metadata
                    if (Metadata != null)
                    {
                        UInt24 metaDataLength = new UInt24((uint) Metadata.Count);
                        bytes.AddRange(metaDataLength.BytesBigEndian);
                        bytes.AddRange(Metadata);
                    }
                    else
                    {
                        // Write zero length if we set flag but didn't provide data
                        bytes.Add(0);
                    }
                }

                if (Data == null) return;

                // Remainder of the frame is assumed to be data, no need to length prefix
                bytes.AddRange(Data);
            }

            private void WriteHeader(List<byte> bytes)
            {
                WriteIntBigEndian(bytes, BitConverter.GetBytes(MajorVersion));
                WriteIntBigEndian(bytes, BitConverter.GetBytes(MinorVersion));
                WriteIntBigEndian(bytes, BitConverter.GetBytes(KeepAlive));
                WriteIntBigEndian(bytes, BitConverter.GetBytes(LifeTime));

                // TODO: handle resume token
                // - (16 bits = max value 65,535) Unsigned 16-bit integer of Resume Identification Token Length in bytes. (Not present if R flag is not set)
                // - Token used for client resume identification (Not present if R flag is not set)
                // const ushort resumeTokenLength = 0;
                // WriteBytes(bytes, BitConverter.GetBytes(resumeTokenLength));

                // MetadataMimeType length (uint8)
                byte metaDataMimeTypeLength = (byte) (MetadataMimeType != null
                    ? Encoding.ASCII.GetByteCount(MetadataMimeType)
                    : 0);
                byte[] metadataMimeTypeLengthBytes = {metaDataMimeTypeLength};
                bytes.AddRange(metadataMimeTypeLengthBytes);
                // WriteBytes(bytes, metadataMimeTypeLengthBytes);
                if (MetadataMimeType != null)
                {
                    bytes.AddRange(Encoding.ASCII.GetBytes(MetadataMimeType));
                }

                // DataMimeType length (uint8)
                byte dataMimeTypeLength = (byte) (DataMimeType != null
                    ? Encoding.UTF8.GetByteCount(DataMimeType)
                    : 0);
                byte[] dataMimeTypeLengthBytes = {dataMimeTypeLength};
                bytes.AddRange(dataMimeTypeLengthBytes);
                // WriteBytes(bytes, dataMimeTypeLengthBytes);
                if (DataMimeType != null)
                {
                    bytes.AddRange(Encoding.ASCII.GetBytes(DataMimeType));
                }
            }

            private void WriteCommonHeader(List<byte> bytes)
            {
                WriteStreamId(bytes);
                WriteTypeAndFlags(bytes);
            }

            public SetupFrame Deserialize(byte[] bytes)
            {
                throw new NotImplementedException();
            }

            public List<byte> SerializeLengthPrefixed()
            {
                List<byte> bytes = Serialize();
                List<byte> lengthPrefixed = new List<byte>();

                UInt24 length = new UInt24((uint) bytes.Count);
                IEnumerable<byte> lengthAsBytes = length.BytesBigEndian;

                lengthPrefixed.AddRange(lengthAsBytes);
                lengthPrefixed.AddRange(bytes);

                return lengthPrefixed;
            }
        }
    }
}