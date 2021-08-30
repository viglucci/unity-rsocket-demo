using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace RSocket
{
    public interface IPayload
    {
        public List<byte> Data { get; }

        public List<byte> Metadata { get; }
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
        public void OnNext(IPayload payload, bool isComplete);
        
        public void OnComplete();

        public void OnError(Exception error);
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

    public interface ISubscriberExtensionSubscriberUnionWithSubscription : ISubscriber, IExtensionSubscriber,
        ISubscription
    {
    }

    public interface IRSocket
    {
        public ICancellable FireAndForget(IPayload payload, [NotNull] ISubscriber responderStream);

        public IExtensionSubscriberWithCancellation RequestResponse(IPayload payload, ISubscriber responderStream);

        public ISubscriptionWithExtensionSubscriber RequestStream(IPayload payload, int initialRequestN,
            ISubscriber responderStream);

        public ISubscriberExtensionSubscriberUnionWithSubscription
            RequestChannel(IPayload payload, int initialRequestN, bool isComplete,
                ISubscriber responderStream);
    }

    public interface IClientTransport
    {
        public void Connect(Action<IDuplexConnection, Exception> callback);
    }

    public interface IDuplexConnection : ICloseable, IMultiplexer
    {
    }

    public interface IMultiplexer
    {
        IOutboundConnection ConnectionOutbound { get; }

        public void CreateRequestStream(IStreamFrameStreamLifecyleHandler frameHandler);
    }

    public interface IDemultiplexer
    {
        public void ConnectionInBound(Action<RSocketFrame.Frame> handler);
    }

    public interface IFrameHandler
    {
        public void Handle(RSocketFrame.Frame frame);
    }

    public interface IStreamFrameHandler : IFrameHandler
    {
        RSocketFrameType StreamType { get; }

        int StreamId { get; }
    }

    public interface IStreamLifecycleHandle
    {
        public bool HandleReady(int streamId, IStream stream);

        public void HandleReject(Exception exception);
    }

    public interface IStream : IOutboundConnection
    {
        
    }
    
    public interface IStreamFrameStreamLifecyleHandler
        : IStreamFrameHandler, IStreamLifecycleHandle {}
    
    public interface IAvailabilityProvider
    {
    }

    public interface IStreamIdGenerator
    {
        public void Next(Func<int, bool> callback, List<int> streams);
    }

    public interface ICloseable
    {
        public void Close(Exception error);

        public void OnClose(Action<Exception> error);
    }

    public interface IInboundConnection
    {
        public void Handle();
    }

    public interface IOutboundConnection
    {
        public void Send(ISerializableFrame<RSocketFrame.Frame> frame);
    }
}