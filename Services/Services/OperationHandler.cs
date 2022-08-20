using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Services
{
    public class OperationHandler<T> : IOperationHandler<T>, IDisposable where T : class
    {
        #region snippet_Properties

        private readonly Subject<T> _subject;

        private readonly Dictionary<string, IDisposable> _subscribers;

        #endregion

        #region snippet_Constructors

        public OperationHandler()
        {
            _subject = new Subject<T>();
            _subscribers = new Dictionary<string, IDisposable>();
        }

        #endregion

        #region snippet_ActionMethods

        public void Publish(T eventType) => _subject.OnNext(eventType);

        public void Subscribe(string subscriberName, Action<T> action)
        {
            if (!_subscribers.ContainsKey(subscriberName))
            {
                _subscribers.Add(subscriberName, _subject.Subscribe(action));
            }
        }

        public void Dispose()
        {
            if (!(_subject is null)) _subject.Dispose();

            foreach(var subscriber in _subscribers)
            {
                subscriber.Value.Dispose();
            }
        }

        #endregion
    }
}
