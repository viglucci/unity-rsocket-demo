using System;

namespace RSocket
{
    public class RSocketRequester : IRSocket
    {
        private readonly IDuplexConnection _connection;

        public RSocketRequester(IDuplexConnection connection)
        {
            _connection = connection;
        }

        public ICancellable FireAndForget(IPayload payload, ISubscriber responderStream)
        {
            RequestFnFRequesterHandler handler = new RequestFnFRequesterHandler(payload, responderStream);

            _connection.CreateRequestStream(handler);

            return handler;
        }

        public IExtensionSubscriberWithCancellation RequestResponse(IPayload payload, ISubscriber responderStream)
        {
            throw new System.NotImplementedException();
        }

        public ISubscriptionWithExtensionSubscriber RequestStream(IPayload payload, int initialRequestN, ISubscriber responderStream)
        {
            throw new System.NotImplementedException();
        }

        public ISubscriberExtensionSubscriberUnionWithSubscription RequestChannel(IPayload payload, int initialRequestN,
            bool isComplete, ISubscriber responderStream)
        {
            throw new System.NotImplementedException();
        }
    }

    public class RequestFnFRequesterHandler : IStreamFrameStreamLifecyleHandler, ICancellable
    {
        private bool done;
        
        private readonly IPayload _payload;
        private readonly ISubscriber _receiver;
        
        public RSocketFrameType StreamType { get; }
        public int StreamId { get; private set; }

        public RequestFnFRequesterHandler(IPayload payload, ISubscriber receiver)
        {
            _payload = payload;
            _receiver = receiver;
        }

        public void Handle(RSocketFrame.Frame frame)
        {
            throw new System.NotImplementedException();
        }
        
        public void Cancel()
        {
            throw new System.NotImplementedException();
        }

        public bool HandleReady(int streamId, IStream stream)
        {
            if (done)
            {
                return false;
            }
            
            StreamId = streamId;

            stream.Send(new RSocketFrame.RequestFnfFrame(streamId)
            {
                Data = _payload.Data,
                Metadata = _payload.Metadata
            });

            done = true;
            
            _receiver.OnComplete();

            return true;
        }

        public void HandleReject(Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}