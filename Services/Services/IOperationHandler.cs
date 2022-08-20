using System;

namespace Services
{
    public interface IOperationHandler<T> where T : class
    {
        void Publish(T eventType);

        void Subscribe(string subscriberName, Action<T> action);
    }
}
