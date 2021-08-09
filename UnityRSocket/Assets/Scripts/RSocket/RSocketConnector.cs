using System;
using UnityEngine;

namespace RSocket
{
    public class RSocketConnector
    {
        private readonly IClientTransport _clientTransport;

        public RSocketConnector(IClientTransport clientTransport)
        {
            _clientTransport = clientTransport;
        }

        public void Bind(Action<IRSocket> action)
        {
            _clientTransport.Connect((connection, exception) =>
            {
                if (exception != null)
                {
                    Debug.LogError(exception);
                    return;
                }
               
                Debug.Log("Transport connected...");
                Debug.Log(connection);
                
                RSocketFrame.SetupFrame frame = new RSocketFrame.SetupFrame()
                {
                    Type = RSocketFrameType.SETUP,
                    DataMimeType = "application/octet-stream",
                    MetadataMimeType = "application/octet-stream",
                    KeepAlive = 60000,
                    LifeTime = 300000,
                    StreamId = 0,
                    MajorVersion = 1,
                    MinorVersion = 0,
                    Flags = (int) RSocketFlagType.NONE
                };
                Debug.Log("Sending SETUP frame...");
                Debug.Log(frame);
                
                connection.Send(frame);
            });
        }
    }
}