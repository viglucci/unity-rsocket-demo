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

                // Stream ID
                BufferUtils.WriteUInt32BigEndian(bytes, StreamId);
                
                // Type and Flags
                int type = (int) Type << FrameTypeOffset;
                int flags = Flags & FlagsMask;
                Int16 typeAndFlags = (Int16) (type | flags);
                BufferUtils.WriteUInt16BigEndian(bytes, typeAndFlags);

                BufferUtils.WriteUInt16BigEndian(bytes, MajorVersion);
                BufferUtils.WriteUInt16BigEndian(bytes, MinorVersion);
                BufferUtils.WriteUInt32BigEndian(bytes, KeepAlive);
                BufferUtils.WriteUInt32BigEndian(bytes, LifeTime);
                
                // TODO: handle resume token
                // - (16 bits = max value 65,535) Unsigned 16-bit integer of Resume Identification Token Length in bytes. (Not present if R flag is not set)
                // - Token used for client resume identification (Not present if R flag is not set)
                // const ushort resumeTokenLength = 0;
                // WriteBytes(bytes, BitConverter.GetBytes(resumeTokenLength));

                // MetadataMimeType length (uint8)
                byte metaDataMimeTypeLength = (byte) (MetadataMimeType != null
                    ? Encoding.ASCII.GetByteCount(MetadataMimeType)
                    : 0);
                BufferUtils.WriteInt8(bytes, metaDataMimeTypeLength);
                if (MetadataMimeType != null)
                {
                    // Protocol spec dictates ASCII
                    bytes.AddRange(Encoding.ASCII.GetBytes(MetadataMimeType));
                }

                // DataMimeType length (uint8)
                byte dataMimeTypeLength = (byte) (DataMimeType != null
                    ? Encoding.UTF8.GetByteCount(DataMimeType)
                    : 0);
                BufferUtils.WriteInt8(bytes, dataMimeTypeLength);
                // WriteBytes(bytes, dataMimeTypeLengthBytes);
                if (DataMimeType != null)
                {
                    // Protocol spec dictates ASCII
                    bytes.AddRange(Encoding.ASCII.GetBytes(DataMimeType));
                }
                
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
                        BufferUtils.WriteUInt24BigEndian(bytes, Metadata.Count);
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

            public SetupFrame Deserialize(byte[] bytes)
            {
                throw new NotImplementedException();
            }

            public List<byte> SerializeLengthPrefixed()
            {
                // TODO: improve memory allocation
                List<byte> bytes = Serialize();
                List<byte> lengthPrefixed = new List<byte>();
                BufferUtils.WriteUInt24BigEndian(lengthPrefixed, bytes.Count);
                lengthPrefixed.AddRange(bytes);
                return lengthPrefixed;
            }
        }
    }
}