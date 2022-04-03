using RSocket;
using UnityEngine;

public class TcpTransportProvider : MonoBehaviour, IRSocketTransportProvider
{
    
    [SerializeField] public string host = "localhost";
    [SerializeField] public int port = 9091;

    public IClientTransport Transport { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Transport = new TcpClientTransport(host, port);
    }
}