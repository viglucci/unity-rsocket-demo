using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RSocket;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    private static ClientManager _instance;

    [SerializeField] public string host;
    [SerializeField] public int port;
    private IRSocket _rSocket;

    private IClientTransport _transport;
    private readonly int _wsPort = 7000;

    private void Awake()
    {
        Debug.Log("ClientManager Awake");

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
        Debug.Log("ClientManager Start");
        
        _transport = new SimpleWebTransportTransport(
            "ws", "localhost", _wsPort, 5000, 20000, 5000);
        // _transport = new TcpClientTransport(host, port);

        SetupOptions setupOptions = new SetupOptions(
            keepAlive: 3000, // 3 seconds
            // keepAlive: 30_000, // 30 second
            lifetime: 30_000, // 30 seconds
            // lifetime: 300_000, // 5 minutes
            data: new List<byte>(),
            metadata: new List<byte>()
            // data: new List<byte>(Encoding.ASCII.GetBytes("This could be anything")),
            // metadata: new List<byte>(Encoding.ASCII.GetBytes("This could also be anything"))
        );
        RSocketConnector connector = new RSocketConnector(
            _transport,
            setupOptions,
            new MonoBehaviorScheduler());

        try
        {
            Debug.Log("ClientManager binding connector");
            _rSocket = connector.Bind();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        OnRSocketConnected();
    }

    private void Update()
    {
        _transport.ProcessMessages();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OnRSocketConnected()
    {
        _rSocket.OnClose((ex) =>
        {
            Debug.Log("RSocket connection closed.");
            if (ex != null)
                Debug.LogError(ex);
        });

        ICancellable cancellable = _rSocket.RequestResponse(new RSocketPayload
            {
                Data = new List<byte>(Encoding.ASCII.GetBytes("PING"))
            },
            new Subscriber(
                (payload, isComplete) =>
                {
                    string decodedData = Encoding.UTF8.GetString(payload.Data.ToArray());
                    string decodedMetadata = Encoding.UTF8.GetString(payload.Metadata.ToArray());

                    Debug.Log($"data: {decodedData}");
                    Debug.Log($"metadata: {decodedMetadata}");
                    Debug.Log($"isComplete: {isComplete}");
                },
                () => Debug.Log("RequestResponse done"),
                (ex) => Debug.LogError(ex)
            ));

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
    // // Better example for how to do Coroutine on interval
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