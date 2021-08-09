using System;
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

        public List<byte> Data
        {
            get => _data;
        }

        public List<byte> Metadata
        {
            get => _metadata;
        }

        public string DataMimeType
        {
            get => _dataMimeType;
        }

        public string MetadataMimeType
        {
            get => _metadataMimeType;
        }

        public int KeepAlive
        {
            get => _keepAlive;
        }

        public int Lifetime
        {
            get => _lifetime;
        }
    }

    public class RSocketConnector
    {
        private readonly IClientTransport _clientTransport;
        private readonly SetupOptions _setupOptions;

        public RSocketConnector(
            IClientTransport clientTransport,
            SetupOptions setupOptions)
        {
            _clientTransport = clientTransport;
            _setupOptions = setupOptions;
        }

        public void Bind(Action<IRSocket> onBound)
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
                    Data = _setupOptions.Data,
                    DataMimeType = _setupOptions.DataMimeType,
                    Metadata = _setupOptions.Metadata,
                    MetadataMimeType = _setupOptions.MetadataMimeType,
                    KeepAlive = _setupOptions.KeepAlive,
                    LifeTime = _setupOptions.Lifetime,
                    StreamId = 0,
                    MajorVersion = 1,
                    MinorVersion = 0,
                    Flags = (ushort) (_setupOptions.Metadata != null ? RSocketFlagType.METADATA : RSocketFlagType.NONE)
                };

                Debug.Log("Sending SETUP frame...");

                connection.Send(frame);

                onBound(new RSocketRequester());
            });
        }
    }
}