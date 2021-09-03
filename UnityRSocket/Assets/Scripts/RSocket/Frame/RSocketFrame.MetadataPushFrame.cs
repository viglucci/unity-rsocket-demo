using System;
using System.Collections.Generic;

namespace RSocket.Frame
{
    public static partial class RSocketFrame
    {
        public class MetadataPushFrame : AbstractFrame
        {
            public override FrameType Type => FrameType.METADATA_PUSH;

            public override List<byte> Serialize()
            {
                throw new NotImplementedException();
            }

            public MetadataPushFrame(int streamId) : base(streamId)
            {
            }
        }
    }
}