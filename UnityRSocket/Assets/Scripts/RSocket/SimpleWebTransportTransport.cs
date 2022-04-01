using System;
using System.Threading.Tasks;
using JamesFrowen.SimpleWeb;

namespace RSocket
{
    public class SimpleWebTransportTransport : IClientTransport
    {
        private readonly string _scheme;
        private readonly string _host;
        private readonly int _port;
        private readonly int _sendTimeout;
        private readonly int _receiveTimeout;
        private readonly int _messagesPerTick;
        private SimpleWebClient _client;

        public SimpleWebTransportTransport(
            string scheme,
            string host,
            int port,
            int sendTimeout,
            int receiveTimeout,
            int messagesPerTick)
        {
            _scheme = scheme;
            _host = host;
            _port = port;
            _sendTimeout = sendTimeout;
            _receiveTimeout = receiveTimeout;
            _messagesPerTick = messagesPerTick;
        }

        public IDuplexConnection Connect()
        {
            TcpConfig tcpConfig =
                new TcpConfig(false, _sendTimeout, _receiveTimeout);

            _client = SimpleWebClient.Create(ushort.MaxValue, _messagesPerTick, tcpConfig);

            UriBuilder builder = new UriBuilder
            {
                Scheme = _scheme,
                Host = _host,
                Port = _port
            };

            SimpleWebTransportDuplexConnection connection
                = new SimpleWebTransportDuplexConnection(_client);

            _client.Connect(builder.Uri);
            
            return connection;
        }

        public void ProcessMessageQueue()
        {
            _client.ProcessMessageQueue();
        }
    }
}