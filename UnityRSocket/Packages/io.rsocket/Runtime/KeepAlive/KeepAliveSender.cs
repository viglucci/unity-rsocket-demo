using RSocket.Frame;

namespace RSocket.KeepAlive
{
    public class KeepAliveSender
    {
        private readonly IOutboundConnection _outboundConnection;
        private readonly int _keepAlivePeriodDurationMillis;
        private readonly IScheduler _scheduler;

        public KeepAliveSender(
            IOutboundConnection outboundConnection,
            int keepAlivePeriodDurationMillis,
            IScheduler scheduler)
        {
            _outboundConnection = outboundConnection;
            _keepAlivePeriodDurationMillis = keepAlivePeriodDurationMillis;
            _scheduler = scheduler;
        }

        public void Start()
        {
            int timeSeconds = _keepAlivePeriodDurationMillis / 1000;
            _scheduler.StartInterval(timeSeconds, SendKeepAlive);
        }

        private void SendKeepAlive()
        {
            RSocketFrame.KeepAliveFrame keepAliveFrame = new RSocketFrame.KeepAliveFrame(0)
            {
                Flags = (ushort)RSocketFlagType.RESPOND
            };
            _outboundConnection.Send(keepAliveFrame);
        }
    }
}