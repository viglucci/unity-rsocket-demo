using UnityEngine;
using Viglucci.UnityRSocket;
using Viglucci.UnityRSocketTransportWebSocket;

public class WebSocketTransportProvider : MonoBehaviour, IRSocketTransportProvider
{
    public string scheme = "ws";
    public string localhost = "localhost";
    public int port = 7000;
    public int sendTimeout = 5000;
    public int receiveTimeout = 20000;
    public int messagesPerTick = 5000;
    private WebsocketTransport _transport;

    // Start is called before the first frame update
    void Awake()
    {
        _transport = new WebsocketTransport(
            scheme, localhost, port, sendTimeout, receiveTimeout, messagesPerTick);
    }

    public IClientTransport GetTransport()
    {
        return _transport;
    }
}