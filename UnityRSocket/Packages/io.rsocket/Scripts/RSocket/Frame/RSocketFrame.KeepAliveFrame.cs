using System;
using System.Collections.Generic;

namespace RSocket.Frame
{
    public static partial class RSocketFrame
    {
        public class KeepAliveFrame : AbstractFrame
        {
            public override FrameType Type => FrameType.KEEPALIVE;

            public override List<byte> Serialize()
            {
                throw new NotImplementedException();
            }

            public KeepAliveFrame(int streamId) : base(streamId)
            {
            }
        }
    }
}