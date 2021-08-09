using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

namespace RSocket
{
    public class TcpClientTransport : IClientTransport
    {
        private readonly string _host;
        private readonly int _port;
        private TcpClient _socket;

        private static int DataBufferSize = 4096;

        public TcpClientTransport(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public void Connect(Action<IDuplexConnection, Exception> callback)
        {
            _socket = new TcpClient()
            {
                ReceiveBufferSize = DataBufferSize,
                SendBufferSize = DataBufferSize
            };

            _socket.BeginConnect(_host, _port, ar =>
            {
                try
                {
                    _socket.EndConnect(ar);
                    
                    if (!_socket.Connected)
                    {
                        return;
                    }

                    TcpDuplexConnection connection = new TcpDuplexConnection(_socket);
                    callback(connection, null);
                }
                catch(Exception e)
                {
                    Debug.LogError($"Failed to connect on TCP: {e}");
                }
            }, _socket);
        }

        private class TcpDuplexConnection : IDuplexConnection
        {
            private readonly NetworkStream _stream;
            private readonly byte[] _receiveBuffer;

            public TcpDuplexConnection(TcpClient socket)
            {
                _receiveBuffer = new byte[DataBufferSize];
                _stream = socket.GetStream();
                _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, OnData, null);
            }

            public void Handle()
            {
                throw new NotImplementedException();
            }

            private void OnData(IAsyncResult ar)
            {
                try
                {
                    int byteLength = _stream.EndRead(ar);
                    if (byteLength <= 0)
                    {
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(_receiveBuffer, data, byteLength);
                    HandleData(data);

                    _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, OnData, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            private void HandleData(byte[] data)
            {
                throw new NotImplementedException();
            }

            public void Send(ISerializable<RSocketFrame.SetupFrame> frame)
            {
                try
                {
                    var serialized = frame.SerializeLengthPrefixed();

                    _stream.BeginWrite(serialized.ToArray(), 0, serialized.Count, (ar) =>
                    {
                        _stream.EndWrite(ar);
                    }, null);
                }
                catch (Exception ex)
                {
                    Debug.Log($"Error sending data to server via TCP: {ex}");
                }
            }
        }
    }
}