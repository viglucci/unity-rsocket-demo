using System;
using System.Collections.Generic;

namespace RSocket.Frame
{
    public static partial class RSocketFrame
    {
        public class UnsupportedFrame : AbstractFrame
        {
            public override FrameType Type => FrameType.ERROR;

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