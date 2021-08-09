using System;
using System.Collections.Generic;

namespace RSocket
{
    public interface IPayload
    {
        public List<byte> Data { get; }

        public List<byte> MetaData { get; }
    }

    public interface ICancellable
    {
        public void Cancel();
    }

    public interface ISubscription : ICancellable
    {
        public void Request(int requestN);
    }

    public interface ISubscriber
    {
        public void OnError(Exception error);

        public void OnNext(IPayload payload);
    }

    public interface IExtensionSubscriber
    {
        public void OnExtension(int extendedType, List<byte> content, bool canBeIgnored);
    }

    public interface IExtensionSubscriberWithCancellation : ICancellable, IExtensionSubscriber
    {
    }
    
    public interface ISubscriptionWithExtensionSubscriber : ISubscription, IExtensionSubscriber
    {
    }
    
    public interface ISubscriberExtensionSubscriberUnionWithSubscription : ISubscriber, IExtensionSubscriber, ISubscription
    {
    }
    
    public interface IRSocket
    {
        public ICancellable FireAndForget(IPayload payload, ISubscriber responderStream);

        public IExtensionSubscriberWithCancellation RequestResponse(IPayload payload, ISubscriber responderStream);

        public ISubscriptionWithExtensionSubscriber RequestStream(IPayload payload, int initialRequestN, ISubscriber responderStream);

        public ISubscriberExtensionSubscriberUnionWithSubscription
            RequestChannel(IPayload payload, int initialRequestN, bool isComplete,
            ISubscriber responderStream);
    }
    
    public interface IClientTransport
    {
        public void Connect(Action<IDuplexConnection, Exception> callback);
    }

    public interface IDuplexConnection : IOutboundConnection, IInboundConnection, ICloseable, IAvailabilityProvider
    {
    }

    public interface IAvailabilityProvider
    {
    }

    public interface ICloseable
    {
    }

    public interface IInboundConnection
    {
        public void Handle();
    }

    public interface IOutboundConnection
    {
        public void Send(ISerializable<RSocketFrame.SetupFrame> frame);
    }
}