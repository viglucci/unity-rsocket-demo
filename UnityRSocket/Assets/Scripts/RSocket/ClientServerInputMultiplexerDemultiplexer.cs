using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RSocket
{
    public abstract class ClientServerInputMultiplexerDemultiplexer : Deferred, IMultiplexer, IStream
    {
        private readonly IStreamIdGenerator _streamIdGenerator;

        public IOutboundConnection ConnectionOutbound { get; }

        private readonly Dictionary<int, IStreamFrameHandler> _streamFrameHandlers = new Dictionary<int, IStreamFrameHandler>();
        
        public ClientServerInputMultiplexerDemultiplexer(IStreamIdGenerator streamIdGenerator)
        {
            _streamIdGenerator = streamIdGenerator;
        }

        protected void Handle(RSocketFrame.Frame frame)
        {
            if (frame.Type == RSocketFrameType.RESERVED)
            {
                // TODO: throw
                return;
            }

            if (RSocketFrameUtils.IsConnectionFrame(frame))
            {
                // TODO: Connection stream handling
                throw new NotImplementedException();
            }

            if (RSocketFrameUtils.IsRequestFrame(frame))
            {
                // TODO: Request stream handling
                throw new NotImplementedException();
            }

            _streamFrameHandlers.TryGetValue(frame.StreamId, out IStreamFrameHandler handler);

            if (handler == null)
            {
                Debug.LogWarning($"Failed to find handler for unknown stream {frame.StreamId} for frame type ${frame.Type}.");
                return;
            }
            
            handler.Handle(frame);

            throw new NotImplementedException();
        }
        
        public void CreateRequestStream(IStreamFrameStreamLifecyleHandler streamHandler)
        {
            if (Done)
            {
                streamHandler.HandleReject(new Exception("Stream already closed."));
                return;
            }

            _streamIdGenerator.Next((streamId) =>
            {
                _streamFrameHandlers.Add(streamId, streamHandler);
                return streamHandler.HandleReady(streamId, this);
            }, _streamFrameHandlers.Keys.ToList());
        }
        
        public abstract void Send(ISerializableFrame<RSocketFrame.Frame> frame);
    }
}