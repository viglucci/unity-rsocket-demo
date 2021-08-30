using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSocket
{
    public class SetupOptions
    {
        private List<byte> _data;
        private List<byte> _metadata;
        private string _dataMimeType;
        private string _metadataMimeType;
        private int _keepAlive;
        private int _lifetime;

        public SetupOptions(int keepAlive, int lifetime,
            List<byte> metadata,
            List<byte> data,
            string metadataMimeType = "application/octet-stream",
            string dataMimeType = "application/octet-stream")
        {
            _lifetime = lifetime;
            _keepAlive = keepAlive;
            _metadataMimeType = metadataMimeType;
            _dataMimeType = dataMimeType;
            _metadata = metadata;
            _data = data;
        }

        public List<byte> Data => _data;

        public List<byte> Metadata => _metadata;

        public string DataMimeType => _dataMimeType;

        public string MetadataMimeType => _metadataMimeType;

        public int KeepAlive => _keepAlive;

        public int Lifetime => _lifetime;
    }

    public class RSocketConnector
    {
        private readonly IClientTransport _clientTransport;
        private readonly RSocketFrame.SetupFrame _setupFrame;

        public RSocketConnector(
            IClientTransport clientTransport,
            SetupOptions setupOptions)
        {
            _clientTransport = clientTransport;

            ushort metaDataFlag = (ushort) (setupOptions.Metadata != null
                ? RSocketFlagType.METADATA
                : RSocketFlagType.NONE);

            _setupFrame = new RSocketFrame.SetupFrame(0)
            {
                Data = setupOptions.Data,
                DataMimeType = setupOptions.DataMimeType,
                Metadata = setupOptions.Metadata,
                MetadataMimeType = setupOptions.MetadataMimeType,
                KeepAlive = setupOptions.KeepAlive,
                LifeTime = setupOptions.Lifetime,
                MajorVersion = 1,
                MinorVersion = 0,
                Flags = metaDataFlag
            };
        }

        public IEnumerator Bind(Action<IRSocket> onBound)
        {
            _clientTransport.Connect((connection, exception) =>
            {
                if (exception != null)
                {
                    Debug.LogError(exception);
                    return;
                }

                Debug.Log("Transport connected...");

                // ConnectionFrameHandler connectionFrameHandler = new ConnectionFrameHandler(connection);

                Debug.Log("Sending SETUP frame...");
                connection.ConnectionOutbound.Send(_setupFrame);

                onBound(new RSocketRequester(connection));
            });

            yield return null;
        }
    }

    public class ConnectionFrameHandler
    {
        public ConnectionFrameHandler(IDuplexConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}