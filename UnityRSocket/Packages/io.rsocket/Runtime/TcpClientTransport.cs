using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace RSocket
{
    public class TcpClientTransport : IClientTransport
    {
        private readonly string _host;
        private readonly int _port;
        private TcpClient _socket;

        public TcpClientTransport(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public IDuplexConnection Connect()
        {
            _socket = new TcpClient()
            {
                ReceiveBufferSize = TcpDuplexConnection.DataBufferSize,
                SendBufferSize = TcpDuplexConnection.DataBufferSize
            };
            
            // TODO: fix Connect for TCP without requiring connect to be async
            // IAsyncResult asyncResult = _socket.BeginConnect(_host, _port, (ar) => {}, _socket);
            // await Task.Factory.FromAsync(asyncResult, _socket.EndConnect);
            
            return new TcpDuplexConnection(_socket);
        }
        
        public IDuplexConnection ConnectSync()
        {
            throw new NotImplementedException();
        }
    }
}