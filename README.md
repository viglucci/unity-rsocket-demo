# unity-rsocket

> 🚨 This project is a work in progress, and not yet available on Unity Hub.

`unity-rsocket` is a Unity compatible implementation of [RSocket](https://rsocket.io).

## Feature Support

### RSocket Core Features Support

- 🟨 Request Interactions (partial support)
- ✔️ TCP Client Transport
- ❌ WebSocket Client Transport
- ❌ Leasing
- ❌ Resumability
- ✔️ Keepalive
- 🟨 Composite Metadata (wip)
- ✔️ Routing

### Request Models

- ✔️ Fire and Forget
- ✔️ Request Response
- ✔️ Request Stream
- ❌ Request Channel
- ❌ Metadata Push

## Examples

### Create an RSocket connector

```c#
private async void Start()
{
    string host = "tcp://localhost";
    int port = "9090";

    IClientTransport transport = new TcpClientTransport(host, port);
    
    SetupOptions setupOptions = new SetupOptions(
        keepAlive: 60000,
        lifetime: 300000,
        data: new List<byte>(Encoding.ASCII.GetBytes("{\"key\": \"value\"}")),
        metadata: new List<byte>(Encoding.ASCII.GetBytes("{\"key\": \"value\"}")));
    
    RSocketConnector connector = new RSocketConnector(transport, setupOptions);

    Exception connectionException = null;

    IRsocket rSocket;

    try
    {
        rSocket = await connector.Bind();
    }
    catch (Exception e)
    {
        Debug.LogError(e);
        connectionException = e;
    }

    if (connectionException != null) return;

    Debug.Log("RSocket requester bound");
}
```

### Request Response

```c#
ICancellable cancellable = rSocket.RequestResponse(new RSocketPayload
    {
        Data = new List<byte>(Encoding.ASCII.GetBytes("PING"))
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
```

### Request Stream

```c#
int initialRequestN = 100;

_rSocket.RequestStream(new RSocketPayload
    {
        Data = new List<byte>(Encoding.ASCII.GetBytes("PING"))
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
                Debug.Log("RequestStream done");
            }
        },
        () => Debug.Log("RequestStream done"),
        (error) => Debug.LogError($"[code: {error.Code}, message: {error.Message}]", this)
    ),
    initialRequestN);
```

## FAQ

### Why not use rsocket-net?

In our tests, [RSocket-Net](https://github.com/rsocket/rsocket-net) appeared to be incompatible with the Unity scripting environment and available C# APIs. With that being said, if you can get RSocket-Net to work within Unity, we would love to hear about it!

## Known Issues

### Performance

Library has not been performance profiled. Hot paths may not be optimized.

### Memory Usage

Memory allocation has not been profiled for performance consideration. Frame creation implementation required additional work to prevent wasteful memory allocation.

### Keep Alive

The client implementation does not currently send or respond to keep alive messages. As such, a properly written server will terminate the connection after the configured period provided in the setup payload has passed.
