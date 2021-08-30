using System;

namespace RSocket
{
    public class Subscriber : ISubscriber
    {
        private readonly Action<RSocketPayload, bool> _onNext;
        private readonly Action _onComplete;
        private readonly Action<Exception> _onError;

        public Subscriber(
            Action<RSocketPayload, bool> onNext,
            Action onComplete,
            Action<Exception> onError)
        {
            _onNext = onNext;
            _onComplete = onComplete;
            _onError = onError;
        }

        public void OnNext(IPayload payload, bool isComplete)
        {
            throw new NotImplementedException();
        }

        public void OnComplete()
        {
            _onComplete.Invoke();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}