namespace RSocket
{
    public class RSocketRequester : IRSocket
    {
        public ICancellable FireAndForget(IPayload payload, ISubscriber responderStream)
        {
            throw new System.NotImplementedException();
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
}