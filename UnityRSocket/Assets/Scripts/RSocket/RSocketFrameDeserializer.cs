using System;
using System.Collections.Generic;

namespace RSocket
{
    public class RSocketFrameDeserializer
    {
        public static List<(RSocketFrame.Frame, int)> DeserializeFrames(List<byte> bytes)
        {
            List<(RSocketFrame.Frame, int)> frames = new List<(RSocketFrame.Frame, int)>();
            int offset = 0;
            const int uint24Size = 3;
            while (offset + uint24Size < bytes.Count)
            {
                (int frameLength, _) = BufferUtils.ReadUInt24BigEndian(bytes, offset);
                int frameStart = offset + uint24Size;
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
            int type = (int) ((uint) typeAndFlags.value >> RSocketFlagUtils.FrameTypeOffset);
            int flags = typeAndFlags.value & RSocketFlagUtils.FlagsMask;
            return (RSocketFrameType) type switch
            {
                RSocketFrameType.PAYLOAD => DeserializePayloadFrame(frameBuffer, streamId, flags),
                _ => throw new System.NotImplementedException()
            };
        }

        private static RSocketFrame.Frame DeserializePayloadFrame(List<byte> frameBuffer, (int value, int nextOffset) streamId, int flags)
        {
            throw new NotImplementedException();
        }
    }
}