using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace RSocket
{
    public class TcpDuplexConnection : ClientServerInputMultiplexerDemultiplexer, IDuplexConnection
    {
        public static int DataBufferSize = 4096;

        private readonly NetworkStream _stream;
        private readonly byte[] _receiveBuffer;

        public IOutboundConnection ConnectionOutbound => this;

        public TcpDuplexConnection(TcpClient socket)
            : base(streamIdGenerator: StreamIdGenerator.Create(-1))
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

        public override void Send(ISerializableFrame<RSocketFrame.Frame> frame)
        {
            List<byte> bytes = frame.SerializeLengthPrefixed();
            Send(bytes);
        }

        private void Send(List<byte> bytes)
        {
            try
            {
                void AsyncCallback(IAsyncResult ar)
                {
                    _stream.EndWrite(ar);
                }

                _stream.BeginWrite(bytes.ToArray(), 0, bytes.Count, AsyncCallback, null);
            }
            catch (Exception ex)
            {
                Debug.Log($"Error sending data to server via TCP: {ex}");
            }
        } 
    }
}