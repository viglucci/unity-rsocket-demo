using System;
using System.Collections.Generic;
using System.Text;

namespace RSocket
{
    public interface ISerializableFrame<out T>
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
            public int StreamId { get; private set; }
            public ushort Flags { get; set; }

            protected Frame(int streamId)
            {
                StreamId = streamId;
            }

            public abstract RSocketFrameType Type { get; }

            public abstract List<byte> Serialize();

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

        public abstract class RequestFrame : Frame
        {
            protected RequestFrame(int streamId) : base(streamId)
            {
            }

            public List<byte> Data;
            public List<byte> Metadata;

            protected void WritePayload(List<byte> bytes)
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
        }

        public class CancelFrame : Frame
        {
            public override RSocketFrameType Type => RSocketFrameType.CANCEL;

            public override List<byte> Serialize()
            {
                throw new NotImplementedException();
            }

            public CancelFrame(int streamId) : base(streamId)
            {
            }
        }

        public class ErrorFrame : Frame
        {
            public override RSocketFrameType Type => RSocketFrameType.ERROR;

            public override List<byte> Serialize()
            {
                throw new NotImplementedException();
            }

            public ErrorFrame(int streamId) : base(streamId)
            {
            }
        }

        public class KeepAliveFrame : Frame
        {
            public override RSocketFrameType Type => RSocketFrameType.KEEPALIVE;

            public override List<byte> Serialize()
            {
                throw new NotImplementedException();
            }

            public KeepAliveFrame(int streamId) : base(streamId)
            {
            }
        }

        public class LeaseFrame : Frame
        {
            public override RSocketFrameType Type => RSocketFrameType.LEASE;

            public override List<byte> Serialize()
            {
                throw new NotImplementedException();
            }

            public LeaseFrame(int streamId) : base(streamId)
            {
            }
        }

        public class PayloadFrame : RequestFrame
        {
            public override RSocketFrameType Type => RSocketFrameType.PAYLOAD;

            public override List<byte> Serialize()
            {
                throw new NotImplementedException();
            }

            public PayloadFrame(int streamId) : base(streamId)
            {
            }
        }

        public class MetadataPushFrame : Frame
        {
            public override RSocketFrameType Type => RSocketFrameType.METADATA_PUSH;

            public override List<byte> Serialize()
            {
                throw new NotImplementedException();
            }

            public MetadataPushFrame(int streamId) : base(streamId)
            {
            }
        }

        public class RequestChannelFrame : Frame
        {
            public override RSocketFrameType Type => RSocketFrameType.REQUEST_CHANNEL;

            public override List<byte> Serialize()
            {
                throw new NotImplementedException();
            }

            public RequestChannelFrame(int streamId) : base(streamId)
            {
            }
        }

        public class RequestFnfFrame : RequestFrame, ISerializableFrame<RequestFnfFrame>
        {
            public override RSocketFrameType Type => RSocketFrameType.REQUEST_FNF;

            public RequestFnfFrame(int streamId) : base(streamId)
            {
            }
            
            public override List<byte> Serialize()
            {
                List<byte> bytes = new List<byte>();

                // Stream ID
                BufferUtils.WriteUInt32BigEndian(bytes, StreamId);

                // Type and Flags
                int type = (int) Type << FrameTypeOffset;
                int flags = Flags & FlagsMask;
                Int16 typeAndFlags = (Int16) (type | flags);
                BufferUtils.WriteUInt16BigEndian(bytes, typeAndFlags);

                WritePayload(bytes);

                return bytes;
            }

            public RequestFnfFrame Deserialize(byte[] bytes)
            {
                throw new NotImplementedException();
            }
        }

        public class RequestNFrame : RequestFrame, ISerializableFrame<RequestNFrame>
        {
            public override RSocketFrameType Type => RSocketFrameType.REQUEST_N;

            public RequestNFrame(int streamId) : base(streamId)
            {
            }
            
            public override List<byte> Serialize()
            {
                throw new NotImplementedException();
            }

            public RequestNFrame Deserialize(byte[] bytes)
            {
                throw new NotImplementedException();
            }
        }

        public class RequestResponseFrame : RequestFrame, ISerializableFrame<RequestResponseFrame>
        {
            public override RSocketFrameType Type => RSocketFrameType.REQUEST_RESPONSE;

            public RequestResponseFrame(int streamId) : base(streamId)
            {
            }
            
            public override List<byte> Serialize()
            {
                List<byte> bytes = new List<byte>();

                // Stream ID
                BufferUtils.WriteUInt32BigEndian(bytes, StreamId);

                // Type and Flags
                int type = (int) Type << FrameTypeOffset;
                int flags = Flags & FlagsMask;
                Int16 typeAndFlags = (Int16) (type | flags);
                BufferUtils.WriteUInt16BigEndian(bytes, typeAndFlags);

                WritePayload(bytes);

                return bytes;
            }

            public RequestResponseFrame Deserialize(byte[] bytes)
            {
                throw new NotImplementedException();
            }
        }

        public class RequestStreamFrame : Frame
        {
            public override RSocketFrameType Type => RSocketFrameType.REQUEST_STREAM;

            public override List<byte> Serialize()
            {
                throw new NotImplementedException();
            }

            public RequestStreamFrame(int streamId) : base(streamId)
            {
            }
        }

        public class ResumeFrame : Frame
        {
            public override RSocketFrameType Type => RSocketFrameType.RESUME;


            public override List<byte> Serialize()
            {
                throw new NotImplementedException();
            }

            public ResumeFrame(int streamId) : base(streamId)
            {
            }
        }

        public class ResumeOkFrame : Frame
        {
            public override RSocketFrameType Type => RSocketFrameType.RESUME_OK;


            public override List<byte> Serialize()
            {
                throw new NotImplementedException();
            }

            public ResumeOkFrame(int streamId) : base(streamId)
            {
            }
        }

        public class SetupFrame : RequestFrame, ISerializableFrame<SetupFrame>
        {
            public override RSocketFrameType Type => RSocketFrameType.SETUP;

            public string DataMimeType;
            public string MetadataMimeType;
            public int KeepAlive;
            public int LifeTime;
            public ushort MajorVersion;
            public ushort MinorVersion;

            public SetupFrame(int streamId) : base(streamId)
            {
            }

            public override List<byte> Serialize()
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

            public SetupFrame Deserialize(byte[] bytes)
            {
                throw new NotImplementedException();
            }
        }

        public class ExtFrame : Frame
        {
            public override RSocketFrameType Type => RSocketFrameType.EXT;

            public override List<byte> Serialize()
            {
                throw new NotImplementedException();
            }

            public ExtFrame(int streamId) : base(streamId)
            {
            }
        }

        public class UnsupportedFrame : Frame
        {
            public override RSocketFrameType Type => RSocketFrameType.ERROR;

            public override List<byte> Serialize()
            {
                throw new NotImplementedException();
            }

            public UnsupportedFrame(int streamId) : base(streamId)
            {
            }
        }
    }
}