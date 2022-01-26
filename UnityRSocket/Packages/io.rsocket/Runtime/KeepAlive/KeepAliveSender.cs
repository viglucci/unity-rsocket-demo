using System;
using System.Collections;
using RSocket.Frame;
using UnityEngine;

namespace RSocket.KeepAlive
{
    public class KeepAliveSender
    {
        private readonly IOutboundConnection _outboundConnection;
        private readonly int _keepAlivePeriodDurationMillis;
        private readonly MonoBehaviour _monoBehaviour;

        public KeepAliveSender(
            IOutboundConnection outboundConnection,
            int keepAlivePeriodDurationMillis,
            MonoBehaviour monoBehaviour)
        {
            _outboundConnection = outboundConnection;
            _keepAlivePeriodDurationMillis = keepAlivePeriodDurationMillis;
            _monoBehaviour = monoBehaviour;
        }

        public void Start()
        {
            SendKeepAlive();
            ScheduleFrame();
        }

        private void ScheduleFrame()
        {
            int timeSeconds = _keepAlivePeriodDurationMillis / 1000;
            _monoBehaviour.StartCoroutine(DoAfterSeconds(timeSeconds, () =>
            {
                SendKeepAlive();
                ScheduleFrame();
            }));
        }

        private void SendKeepAlive()
        {
            RSocketFrame.KeepAliveFrame keepAliveFrame = new RSocketFrame.KeepAliveFrame(0)
            {
                Flags = (ushort)RSocketFlagType.RESPOND
            };
            _outboundConnection.Send(keepAliveFrame);
        }

        private IEnumerator DoAfterSeconds(float seconds, Action callback)
        {
            yield return new WaitForSeconds(seconds);

            callback.Invoke();
        }
    }
}