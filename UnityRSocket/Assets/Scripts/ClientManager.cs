using System;
using System.Collections;
using System.Collections.Generic;
using RSocket;
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
            // TODO: actually implement keep alive handler in connector...
            // keepAlive: 30000, // 30 seconds
            keepAlive: 3000, // 3 seconds
            lifetime: 600000, // 10 minutes
            data: new List<byte>(),
            metadata: new List<byte>()
            // data: new List<byte>(Encoding.ASCII.GetBytes("This could be anything")),
            // metadata: new List<byte>(Encoding.ASCII.GetBytes("This could also be anything"))
        );
        RSocketConnector connector = new RSocketConnector(
            transport,
            setupOptions,
            this  
        );

        Exception connectionException = null;

        try
        {
            _rSocket = await connector.Bind();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            connectionException = e;
        }

        if (connectionException != null) return;

        Debug.Log("RSocket requester bound");

        OnRSocketConnected();
    }

    private void OnRSocketConnected()
    {
        // ICancellable cancellable = _rSocket.RequestResponse(new RSocketPayload
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
        //                 Debug.Log("RequestResponse done");
        //             }
        //         },
        //         () => Debug.Log("RequestResponse done"),
        //         Debug.LogError
        //     ));
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

    private IEnumerator DoAfterSeconds(float seconds, Action callback)
    {
        yield return new WaitForSeconds(seconds);

        callback.Invoke();
    }
}