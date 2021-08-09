using System.Collections.Generic;
using System.Text;
using RSocket;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    private static ClientManager _instance;

    [SerializeField] public string host;
    [SerializeField] public int port;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Debug.LogWarning(
                $"Instance of {GetType().Name} already exists. Newly created instance will be destroyed.");
            Destroy(this);
        }
    }

    private void Start()
    {
        IClientTransport transport = new TcpClientTransport(host, port);
        SetupOptions setupOptions = new SetupOptions(
            keepAlive: 60000,
            lifetime: 300000,
            data: new List<byte>(Encoding.ASCII.GetBytes("This could be anything")),
            metadata: new List<byte>(Encoding.ASCII.GetBytes("This could also be anything")));
        RSocketConnector connector = new RSocketConnector(transport, setupOptions);

        connector.Bind((rsocket) =>
        {
            Debug.Log("RSocket requester bound");

            // rsocket.FireAndForget(new RSocketPayload()
            // {
            //     Data = new List<byte>(Encoding.ASCII.GetBytes("Hello World"))
            // }, null);
        });
    }
}