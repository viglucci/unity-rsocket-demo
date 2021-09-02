using System;
using System.Collections.Generic;

namespace RSocket
{
    public class RSocketFrameDeserializer
    {
        private const int Uint24Size = 3;

        public static List<(RSocketFrame.Frame, int)> DeserializeFrames(List<byte> bytes)
        {
            List<(RSocketFrame.Frame, int)> frames = new List<(RSocketFrame.Frame, int)>();
            int offset = 0;
            while (offset + Uint24Size < bytes.Count)
            {
                (int frameLength, _) = BufferUtils.ReadUInt24BigEndian(bytes, offset);
                int frameStart = offset + Uint24Size;
                int frameEnd = frameStart + frameLength;
                if (frameEnd > bytes.Count)
                {
                    // not all bytes of next frame received
                    break;
                }

                List<byte> frameBuffer = bytes.GetRange(frameStart, frameLength);
                RSocketFrame.Frame frame = DeserializeFrame(frameBuffer);
                offset = frameEnd;
                frames.Add((frame, offset));
            }

            return frames;
        }

        private static RSocketFrame.Frame DeserializeFrame(List<byte> frameBuffer)
        {
            int offset = 0;
            (int value, int nextOffset) streamId = BufferUtils.ReadUInt32BigEndian(frameBuffer, offset);
            offset = streamId.nextOffset;
            (int value, int nextOffset) typeAndFlags = BufferUtils.ReadUint16BigEndian(frameBuffer, offset);
            int type = (int)((uint)typeAndFlags.value >> RSocketFlagUtils.FrameTypeOffset);
            int flags = typeAndFlags.value & RSocketFlagUtils.FlagsMask;
            offset = typeAndFlags.nextOffset;
            switch ((RSocketFrameType)type)
            {
                case RSocketFrameType.PAYLOAD:
                    return DeserializePayloadFrame(frameBuffer, streamId.value, flags, offset);
                default:
                    throw new NotImplementedException();
            }
        }

        private static RSocketFrame.Frame DeserializePayloadFrame(
            List<byte> frameBuffer,
            int streamId,
            int flags,
            int offset)
        {
            List<byte> metadata = new List<byte>();
            List<byte> data = new List<byte>();

            if (RSocketFlagUtils.HasMetadata(flags))
            {
                (int value, int nextOffset) metadataLength = BufferUtils.ReadUInt24BigEndian(frameBuffer, offset);
                offset = metadataLength.nextOffset;
                if (metadataLength.value > 0)
                {
                    metadata = frameBuffer.GetRange(offset, offset + metadataLength.value);
                    offset += metadataLength.value;
                }
            }

            if (offset < frameBuffer.Count)
            {
                data = frameBuffer.GetRange(offset, frameBuffer.Count - offset);
            }

            return new RSocketFrame.PayloadFrame(streamId)
            {
                Flags = (ushort)flags,
                Metadata = metadata,
                Data = data
            };
        }
    }
}