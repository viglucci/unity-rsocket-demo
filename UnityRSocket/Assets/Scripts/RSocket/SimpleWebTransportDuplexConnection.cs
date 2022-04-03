using System;
using System.Collections.Generic;
using System.Linq;
using JamesFrowen.SimpleWeb;
using RSocket.Frame;

namespace RSocket
{
    public class SimpleWebTransportDuplexConnection : ClientServerInputMultiplexerDemultiplexer, IDuplexConnection
    {
        private readonly SimpleWebClient _client;

        public SimpleWebTransportDuplexConnection(SimpleWebClient _client) : base(StreamIdGenerator.Create(-1))
        {
            this._client = _client;
            this._client.onData += HandleData;
            this._client.onError += HandleError;
            this._client.onDisconnect += HandleClosed;
        }

        public new IOutboundConnection ConnectionOutbound => this;

        public void HandleRequestStream(RSocketStreamHandler handler)
        {
            throw new NotImplementedException();
        }

        public new void Close(Exception e = null)
        {
            _client.Disconnect();
            base.Close(e);
        }

        private void HandleData(ArraySegment<byte> segment)
        {
            List<byte> bytes = (segment.Array ?? throw new InvalidOperationException()).ToList();
            RSocketFrame.AbstractFrame frame
                = FrameDeserializer.DeserializeFrame(bytes.GetRange(segment.Offset, segment.Count));
            Handle(frame);
        }

        private void HandleError(Exception exception)
        {
            Close(new Exception("WebSocket connection error: " + exception.Message));
        }

        private void HandleClosed()
        {
            Close(new Exception("WebSocket connection closed unexpectedly"));
        }

        public override void Send(ISerializableFrame<RSocketFrame.AbstractFrame> frame)
        {
            try
            {
                List<byte> serializedLengthPrefixed = frame.Serialize();
                byte[] bytesArr = serializedLengthPrefixed.ToArray();
                ArraySegment<byte> arraySegment = new ArraySegment<byte>(bytesArr);
                _client.Send(arraySegment);
            }
            catch (Exception ex)
            {
                HandleConnectionError(ex);
            }
        }

        private void HandleConnectionError(Exception exception)
        {
            Close(new Exception("WebSocket connection error: " + exception.Message));
        }
    }
}