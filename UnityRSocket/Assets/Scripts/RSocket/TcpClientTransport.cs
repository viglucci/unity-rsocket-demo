using System;
using System.Net.Sockets;
using UnityEngine;

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

        public void Connect(Action<IDuplexConnection, Exception> callback)
        {
            _socket = new TcpClient()
            {
                ReceiveBufferSize = TcpDuplexConnection.DataBufferSize,
                SendBufferSize = TcpDuplexConnection.DataBufferSize
            };

            _socket.BeginConnect(_host, _port, ar =>
            {
                Exception ex = null;
                
                try
                {
                    _socket.EndConnect(ar);
                    
                    if (!_socket.Connected)
                    {
                        return;
                    }
                }
                catch(Exception e)
                {
                    Debug.LogError($"Failed to connect on TCP: {e}");
                    ex = e;
                }

                if (ex != null)
                {
                    callback(null, ex);
                }
                else
                {
                    TcpDuplexConnection connection = new TcpDuplexConnection(_socket);
                    callback(connection, null);
                }
            }, _socket);
        }
    }
}