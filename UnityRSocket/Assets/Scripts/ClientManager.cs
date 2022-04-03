using System;
using System.Collections.Generic;
using System.Text;
using DTO;
using RSocket;
using RSocket.CompositeMetadata;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    private static ClientManager _instance;

    /**
     * How often to send KeepAlive frames.
     */
    public int keepAliveInterval = 10_000;

    /**
     * The max delay between a keep alive frame and a server ACK. Client will disconnect if server does not
     * respond to a KeepAlive frame within this time period.
     */
    public int keepAliveTimeout = 60_000;

    private IRSocket _rSocket;
    private IClientTransport _transport;

    private void Awake()
    {
        Debug.Log("Awake");

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
        Debug.Log("Start");

        IRSocketTransportProvider rSocketTransportProvider = GetRSocketTransportProvider();

        if (rSocketTransportProvider?.GetTransport() == null)
        {
            throw new Exception(
                "ClientManager must have a IRSocketTransportProvider component. " +
                "Did you forget to add and enable a IRSocketTransportProvider component?");
        }

        _transport = rSocketTransportProvider.GetTransport();

        Debug.Log($"Resolved transport as {_transport.GetType()}");

        SetupOptions setupOptions = new SetupOptions(
            keepAliveInterval, // 3 seconds
            keepAliveTimeout, // 30 seconds
            data: new List<byte>(),
            metadata: new List<byte>(),
            dataMimeType:
            Metadata.WellKnownMimeTypeToString(WellKnownMimeType.APPLICATION_JSON),
            metadataMimeType:
            Metadata.WellKnownMimeTypeToString(WellKnownMimeType.MESSAGE_RSOCKET_COMPOSITE_METADATA)
        );

        RSocketConnector connector = new RSocketConnector(
            _transport,
            setupOptions,
            new MonoBehaviorScheduler());

        try
        {
            Debug.Log("Binding connector");
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

    private IRSocketTransportProvider GetRSocketTransportProvider()
    {
        return GetComponent<IRSocketTransportProvider>();
    }

    private void OnRSocketConnected()
    {
        _rSocket.OnClose((ex) =>
        {
            Debug.Log("RSocket connection closed.");
            if (ex != null)
                Debug.LogError(ex);
        });

        CreateGameRequest createGameRequest = new CreateGameRequest()
        {
            username = "player1"
        };
        string json = JsonUtility.ToJson(createGameRequest);
        List<byte> data = new List<byte>(Encoding.UTF8.GetBytes(json));
        List<byte> metadata = new CompositeMetadataBuilder()
            .WellKnown(
                WellKnownMimeType.MESSAGE_RSOCKET_ROUTING,
                Routing.EncodeRoute("create-game"))
            .Build();

        ICancellable cancellable = _rSocket.RequestResponse(new RSocketPayload
            {
                Data = data,
                Metadata = metadata
            },
            new Subscriber(
                (payload, isComplete) =>
                {
                    string decodedData = Encoding.UTF8.GetString(payload.Data.ToArray());
                    string decodedMetadata = Encoding.UTF8.GetString(payload.Metadata.ToArray());

                    Debug.Log($"data: {decodedData}");
                    Debug.Log($"metadata: {decodedMetadata}");
                    Debug.Log($"isComplete: {isComplete}");

                    if (isComplete)
                    {
                        Debug.Log("RequestResponse done");
                    }
                },
                () => Debug.Log("RequestResponse done"),
                Debug.LogError
            ));


        // _rSocket.FireAndForget(new RSocketPayload
        //     {
        //         Data = new List<byte>(Encoding.UTF8.GetBytes("PING"))
        //     },
        //     new Subscriber(
        //         (payload, isComplete) => throw new NotImplementedException(),
        //         () => Debug.Log("FireAndForget done"),
        //         Debug.LogError
        //     ));

        // _rSocket.RequestStream(new RSocketPayload
        //     {
        //         Data = new List<byte>(Encoding.UTF8.GetBytes("PING"))
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