using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSocket
{
    public class MonoBehaviorScheduler : IScheduler
    {
        private int _nextIntervalId = 0;
        private readonly MonoBehaviour _monoBehaviour;
        private readonly Dictionary<int, Coroutine> _intervals
            = new Dictionary<int, Coroutine>();

        public MonoBehaviorScheduler(MonoBehaviour monoBehaviour)
        {
            _monoBehaviour = monoBehaviour;
        }

        public Coroutine DoAfterSeconds(float seconds, Action callback)
        {
            return _monoBehaviour.StartCoroutine(DoAfterSecondsCo(seconds, callback));
        }

        private IEnumerator DoAfterSecondsCo(float seconds, Action callback)
        {
            yield return new WaitForSeconds(seconds);
        
            callback.Invoke();
        }
        
        public int StartInterval(float seconds, Action callback)
        {
            int intervalId = _nextIntervalId;
            _intervals.Add(intervalId, RunInterval(intervalId, seconds, callback));
            _nextIntervalId += 1;
            return intervalId;
        }

        private Coroutine RunInterval(int intervalId, float seconds, Action callback)
        {
            return DoAfterSeconds(seconds, () =>
            {
                callback.Invoke();
                _intervals[intervalId] = RunInterval(intervalId, seconds, callback);
            });
        }

        public void ClearInterval(int id)
        {
            if (!_intervals.ContainsKey(id))
            {
                return;
            }

            Coroutine value;
            _intervals.TryGetValue(id, out value);
            _monoBehaviour.StopCoroutine(value);
        }
    }
}