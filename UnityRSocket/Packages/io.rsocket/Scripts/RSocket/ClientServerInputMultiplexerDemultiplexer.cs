using System;
using System.Collections.Generic;
using System.Linq;
using RSocket.Frame;
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

        protected void Handle(Frame.RSocketFrame.AbstractFrame abstractFrame)
        {
            if (abstractFrame.Type == FrameType.RESERVED)
            {
                // TODO: throw
                return;
            }

            if (FrameUtils.IsConnectionFrame(abstractFrame))
            {
                // TODO: Connection stream handling
                throw new NotImplementedException();
            }

            if (FrameUtils.IsRequestFrame(abstractFrame))
            {
                // TODO: Request stream handling
                throw new NotImplementedException();
            }

            _streamFrameHandlers.TryGetValue(abstractFrame.StreamId, out IStreamFrameHandler handler);

            if (handler == null)
            {
                Debug.LogWarning($"Failed to find handler for unknown stream {abstractFrame.StreamId} for frame type ${abstractFrame.Type}.");
                return;
            }
            
            handler.Handle(abstractFrame);
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
        
        public abstract void Send(ISerializableFrame<Frame.RSocketFrame.AbstractFrame> frame);
    }
}