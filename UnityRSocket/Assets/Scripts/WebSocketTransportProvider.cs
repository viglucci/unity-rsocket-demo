using RSocket;
using UnityEngine;

public class WebSocketTransportProvider : MonoBehaviour, IRSocketTransportProvider
{
    public string scheme = "ws";
    public string localhost = "localhost";
    public int port = 7000;
    public int sendTimeout = 5000;
    public int receiveTimeout = 20000;
    public int messagesPerTick = 5000;
    private SimpleWebTransportTransport _transport;

    // Start is called before the first frame update
    void Awake()
    {
        _transport = new SimpleWebTransportTransport(
            scheme, localhost, port, sendTimeout, receiveTimeout, messagesPerTick);
    }

    public IClientTransport GetTransport()
    {
        return _transport;
    }
}