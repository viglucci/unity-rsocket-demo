using System.Collections;
using System.Collections.Generic;
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

    public IClientTransport Transport { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        Transport = new SimpleWebTransportTransport(
            scheme, localhost, port, sendTimeout, receiveTimeout, messagesPerTick);
    }
}
