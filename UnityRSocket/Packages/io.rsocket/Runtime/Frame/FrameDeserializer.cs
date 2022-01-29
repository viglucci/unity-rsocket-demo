using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RSocket.Frame
{
    public class FrameDeserializer
    {
        private const int Uint24Size = 3;

        public static IEnumerator<(RSocketFrame.AbstractFrame frame, int offset)> DeserializeFrames(List<byte> bytes)
        {
            int offset = 0;
            while (offset + Uint24Size < bytes.Count)
            {
                (int frameLength, _) = BufferUtils.ReadUInt24BigEndian(bytes, offset);
                int frameStart = offset + Uint24Size;
                int frameEnd = frameStart + frameLength;
                if (frameEnd > bytes.Count)
                {
                    // not all bytes of next frame received
                    yield break;
                }

                List<byte> frameBuffer = bytes.GetRange(frameStart, frameLength);
                RSocketFrame.AbstractFrame abstractFrame = DeserializeFrame(frameBuffer);
                offset = frameEnd;
                yield return (abstractFrame, offset);
            }
        }

        private static RSocketFrame.AbstractFrame DeserializeFrame(List<byte> frameBuffer)
        {
            int offset = 0;
            (int value, int nextOffset) streamId = BufferUtils.ReadUInt32BigEndian(frameBuffer, offset);
            offset = streamId.nextOffset;
            (int value, int nextOffset) typeAndFlags = BufferUtils.ReadUint16BigEndian(frameBuffer, offset);
            int type = (int)((uint)typeAndFlags.value >> RSocketFlagUtils.FrameTypeOffset);
            int flags = typeAndFlags.value & RSocketFlagUtils.FlagsMask;
            offset = typeAndFlags.nextOffset;
            switch ((FrameType)type)
            {
                case FrameType.PAYLOAD:
                    return DeserializePayloadFrame(frameBuffer, streamId.value, flags, offset);
                case FrameType.ERROR:
                    return DeserializeErrorFrame(frameBuffer, streamId.value, flags, offset);
                case FrameType.KEEPALIVE:
                    return DeserializeKeepAliveFrame(frameBuffer, streamId.value, flags, offset);
                default:
                    throw new NotImplementedException();
            }
        }

        private static RSocketFrame.AbstractFrame DeserializeKeepAliveFrame(
            List<byte> frameBuffer,
            int streamId,
            int flags,
            int offset)
        {
            (ulong lastReceivedPosition, int nextOffset) = BufferUtils.ReadUInt64BigEndian(frameBuffer, offset);
            
            // TODO: read data

            return new RSocketFrame.KeepAliveFrame(streamId)
            {
                LastReceivedPosition = lastReceivedPosition
            };
        }

        private static RSocketFrame.AbstractFrame DeserializeErrorFrame(List<byte> frameBuffer, int streamId, int flags,
            int offset)
        {
            (int value, int nextOffset) code = BufferUtils.ReadUInt32BigEndian(frameBuffer, offset);
            offset = code.nextOffset;
            int messageLength = frameBuffer.Count - offset;
            string message = "";
            if (messageLength > 0)
            {
                byte[] messageBytes = frameBuffer.GetRange(offset, messageLength).ToArray();
                message = Encoding.UTF8.GetString(messageBytes);
            }

            return new RSocketFrame.ErrorFrame(streamId, code.value, message);
        }

        private static RSocketFrame.AbstractFrame DeserializePayloadFrame(
            List<byte> frameBuffer,
            int streamId,
            int flags,
            int offset)
        {
            List<byte> metadata = new List<byte>();
            List<byte> data = new List<byte>();

            if (RSocketFlagUtils.HasMetadata(flags))
            {
                (int value, int nextOffset) metadataLength
                    = BufferUtils.ReadUInt24BigEndian(frameBuffer, offset);

                offset = metadataLength.nextOffset;

                if (metadataLength.value > 0)
                {
                    metadata = frameBuffer.GetRange(
                        offset,
                        offset + metadataLength.value);
                    offset += metadataLength.value;
                }
            }

            if (offset < frameBuffer.Count)
            {
                data = frameBuffer.GetRange(
                    offset,
                    frameBuffer.Count - offset);
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