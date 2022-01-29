using System;
using UnityEngine;

namespace RSocket
{
    public interface IScheduler
    {
        public int RunTimeout(float seconds, Action callback);

        public int StartInterval(float seconds, Action callback);

        public void Clear(int id);

        public void RunImmediate(Action callback);
    }
}