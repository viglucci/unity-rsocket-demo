using Viglucci.UnityRSocket;

public interface IRSocketTransportProvider
{
    public IClientTransport GetTransport();
}