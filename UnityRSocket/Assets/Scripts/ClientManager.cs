using System;
using System.Collections.Generic;
using System.Text;
using RSocket;
using RSocket.CompositeMetadata;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    private static ClientManager _instance;

    [SerializeField] public string host;
    [SerializeField] public int port;
    private IRSocket _rSocket;

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

    private async void Start()
    {
        IClientTransport transport = new TcpClientTransport(host, port);
        SetupOptions setupOptions = new SetupOptions(
            keepAlive: 30_000, // 30 seconds
            lifetime: 300_000, // 5 minutes
            data: new List<byte>(),
            metadata: new List<byte>()
            // data: new List<byte>(Encoding.ASCII.GetBytes("This could be anything")),
            // metadata: new List<byte>(Encoding.ASCII.GetBytes("This could also be anything"))
        );
        RSocketConnector connector = new RSocketConnector(
            transport,
            setupOptions,
            new MonoBehaviorScheduler());

        try
        {
            _rSocket = await connector.Bind();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        Debug.Log("RSocket requester bound");

        OnRSocketConnected();
    }

    private void OnRSocketConnected()
    {
        _rSocket.OnClose((ex) =>
        {
            Debug.Log("RSocket connection closed.");
            Debug.LogError(ex);
        });

        Dictionary<string, List<byte>> metaMap = new Dictionary<string, List<byte>>(){
        {
            Metadata.WellKnownMimeTypeToString(WellKnownMimeType.MESSAGE_RSOCKET_ROUTING), new List<byte>()
        }};

        List<byte> data = new List<byte>(Encoding.ASCII.GetBytes("PING"));
        List<byte> metaData = new List<byte>(Encoding.ASCII.GetBytes("PING"));

        ICancellable cancellable = _rSocket.RequestResponse(new RSocketPayload
            {
                Data = data,
                Metadata = metaData
            },
            new Subscriber(
                (payload, isComplete) =>
                {
                    string decodedData = Encoding.UTF8.GetString(payload.Data.ToArray());
                    string decodedMetadata = Encoding.UTF8.GetString(payload.Metadata.ToArray());
        
                    Debug.Log($"[data: {decodedData}, " +
                              $"metadata: {decodedMetadata}, " +
                              $"isComplete: {isComplete}]");
        
                    if (isComplete)
                    {
                        Debug.Log("RequestResponse done");
                    }
                },
                () => Debug.Log("RequestResponse done"),
                Debug.LogError
            ));
        //
        // StartCoroutine(DoAfterSeconds(1.0f, () =>
        // {
        //     Debug.Log("Canceling request response...");
        //     cancellable.Cancel();
        // }));


        // _rSocket.FireAndForget(new RSocketPayload
        //     {
        //         Data = new List<byte>(Encoding.ASCII.GetBytes("PING"))
        //     },
        //     new Subscriber(
        //         (payload, isComplete) => throw new NotImplementedException(),
        //         () => Debug.Log("FireAndForget done"),
        //         Debug.LogError
        //     ));

        // _rSocket.RequestStream(new RSocketPayload
        //     {
        //         Data = new List<byte>(Encoding.ASCII.GetBytes("PING"))
        //     },
        //     new Subscriber(
        //         (payload, isComplete) =>
        //         {
        //             string decodedData = Encoding.UTF8.GetString(payload.Data.ToArray());
        //             string decodedMetadata = Encoding.UTF8.GetString(payload.Metadata.ToArray());
        //
        //             Debug.Log($"[data: {decodedData}, " +
        //                       $"metadata: {decodedMetadata}, " +
        //                       $"isComplete: {isComplete}]");
        //
        //             if (isComplete)
        //             {
        //                 Debug.Log("RequestStream done");
        //             }
        //         },
        //         () => Debug.Log("RequestStream done"),
        //         (error) => { Debug.LogError($"[code: {error.Code}, message: {error.Message}]", this); }
        //     ),
        //     100);
    }

    // private IEnumerator DoAfterSeconds(float seconds, Action callback)
    // {
    //     yield return new WaitForSeconds(seconds);
    //
    //     callback.Invoke();
    // }
    //
    // Better example for how to do Coroutine on interval
    // private IEnumerator DoAndreAfterSeconds(float seconds, Action callback)
    // {
    //     int timesCalled = 0;
    //     
    //     while (true)
    //     {
    //         yield return new WaitForSeconds(seconds);
    //         callback.Invoke();
    //         ++timesCalled;
    //     }
    // }
}