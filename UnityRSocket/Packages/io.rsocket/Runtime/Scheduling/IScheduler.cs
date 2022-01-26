using System;
using UnityEngine;

namespace RSocket
{
    public interface IScheduler
    {
        public Coroutine DoAfterSeconds(float seconds, Action callback);

        public int StartInterval(float seconds, Action callback);

        public void ClearInterval(int id);
    }
}