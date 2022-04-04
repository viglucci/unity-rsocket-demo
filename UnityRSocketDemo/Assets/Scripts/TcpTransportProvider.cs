using UnityEngine;
using Viglucci.UnityRSocket;
using Viglucci.UnityRSocket.Transport;

public class TcpTransportProvider : MonoBehaviour, IRSocketTransportProvider
{
    [SerializeField] public string host = "localhost";
    [SerializeField] public int port = 9091;
    private TcpClientTransport _transport;

    // Start is called before the first frame update
    void Awake()
    {
        _transport = new TcpClientTransport(host, port);
    }

    public IClientTransport GetTransport()
    {
        return _transport;
    }
}