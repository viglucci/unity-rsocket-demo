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
        var connector = new RSocketConnector(new TcpClientTransport(host, port));
        connector.Bind((IRSocket rsocket) =>
        {
            
        });
    }
}