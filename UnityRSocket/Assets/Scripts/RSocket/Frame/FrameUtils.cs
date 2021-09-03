namespace RSocket.Frame
{
    public class FrameUtils
    {
        public static bool IsConnectionFrame(Frame.RSocketFrame.AbstractFrame abstractFrame)
        {
            return abstractFrame.StreamId == 0;
        }
        
        public static bool IsRequestFrame(Frame.RSocketFrame.AbstractFrame abstractFrame)
        {
            return FrameType.REQUEST_RESPONSE <= abstractFrame.Type
                   && abstractFrame.Type <= FrameType.REQUEST_CHANNEL;
        }
    }
}