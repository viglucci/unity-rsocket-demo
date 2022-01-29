using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using RSocket.Frame;
using UnityEngine;

namespace RSocket
{
    public class TcpDuplexConnection : ClientServerInputMultiplexerDemultiplexer, IDuplexConnection
    {
        private readonly TcpClient _socket;
        public static int DataBufferSize = 4096;

        private readonly NetworkStream _stream;
        private readonly byte[] _receiveBuffer;
        private List<byte> _remainingBuffer = new List<byte>();

        public new IOutboundConnection ConnectionOutbound => this;

        public TcpDuplexConnection(TcpClient socket) : base(StreamIdGenerator.Create(-1))
        {
            _socket = socket;
            _receiveBuffer = new byte[DataBufferSize];
            _stream = socket.GetStream();
            _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, OnData, null);
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
                Close(ex);
            }
        }

        private void HandleData(byte[] data)
        {
            List<byte> buffer = _remainingBuffer.Concat(data).ToList();
            int lastOffset = 0;

            IEnumerator<(RSocketFrame.AbstractFrame frame, int offset)> frames
                = FrameDeserializer.DeserializeFrames(buffer);

            while (frames.MoveNext())
            {
                lastOffset = frames.Current.offset;
                Handle(frames.Current.frame);
            }

            _remainingBuffer = new ArraySegment<byte>(data, lastOffset, data.Length - lastOffset).ToList();
        }

        public override void Send(ISerializableFrame<RSocketFrame.AbstractFrame> frame)
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
                    try
                    {
                        _stream.EndWrite(ar);
                    }
                    catch (Exception ex)
                    {
                        HandleConnectionError(ex);
                    }
                }

                _stream.BeginWrite(bytes.ToArray(), 0, bytes.Count, AsyncCallback, null);
            }
            catch (Exception ex)
            {
                HandleConnectionError(ex);
            }
        }

        public void HandleRequestStream(RSocketStreamHandler handler)
        {
            throw new NotImplementedException();
        }

        public void HandleConnectionError(Exception exception)
        {
            Close(new Exception("TCP connection error: " + exception.Message));
        }
    }
}