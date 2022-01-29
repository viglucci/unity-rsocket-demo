using System;
using System.Collections.Generic;
using UnityEngine;

namespace RSocket
{
    public class Deferred : ICloseable
    {
        public bool Done { get; private set; }
        private Exception _exception;
        private readonly List<Action<Exception>> _onCloseCallbacks = new List<Action<Exception>>();

        public void Close(Exception exception = null)
        {
            if (Done)
            {
                Debug.LogWarning("Cannot call close when already done.");
                return;
            }

            Done = true;
            _exception = exception;

            _onCloseCallbacks.ForEach(onCloseCallback =>
            {
                onCloseCallback.Invoke(_exception);
            });
        }

        public void OnClose(Action<Exception> onCloseCallback)
        {
            if (Done)
            {
                onCloseCallback.Invoke(_exception);
                return;
            }
            
            _onCloseCallbacks.Add(onCloseCallback);
        }
    }
}