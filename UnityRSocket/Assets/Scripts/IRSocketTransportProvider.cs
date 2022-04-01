using RSocket;

public interface IRSocketTransportProvider
{
    public abstract IClientTransport Transport { get; }
}