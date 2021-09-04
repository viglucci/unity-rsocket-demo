using System;
using System.Collections.Generic;

namespace RSocket.Frame
{
    public static partial class RSocketFrame
    {
        public class CancelFrame : AbstractFrame
        {
            public override FrameType Type => FrameType.CANCEL;

            public override List<byte> Serialize()
            {
                throw new NotImplementedException();
            }

            public CancelFrame(int streamId) : base(streamId)
            {
            }
        }
    }
}