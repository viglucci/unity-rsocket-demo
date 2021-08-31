using System;
using System.Collections.Generic;
using System.Linq;

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

        public void CreateRequestStream(IExtensionSubscriberWithCancellation frameHandler)
        {
            throw new NotImplementedException();
        }


        public abstract void Send(ISerializableFrame<RSocketFrame.Frame> frame);
    }
}