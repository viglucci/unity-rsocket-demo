namespace RSocket
{
    public class RSocketFrameUtils
    {
        public static bool IsConnectionFrame(RSocketFrame.Frame frame)
        {
            return frame.StreamId == 0;
        }
        
        public static bool IsRequestFrame(RSocketFrame.Frame frame)
        {
            return RSocketFrameType.REQUEST_RESPONSE <= frame.Type
                   && frame.Type <= RSocketFrameType.REQUEST_CHANNEL;
        }
    }
}