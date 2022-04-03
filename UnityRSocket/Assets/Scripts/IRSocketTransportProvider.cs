using RSocket;

public interface IRSocketTransportProvider
{
    public IClientTransport GetTransport();
}