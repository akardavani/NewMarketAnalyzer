using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public abstract class BatchService<T> : IBatchService<T> where T : INotification
    {
        private readonly List<T> _items = [];
        private readonly List<Observer> _observers = [];

        protected BatchService()
        {

        }

        protected List<T> Items => _items;

        public void Add(T item)
        {
            _items.Add(item);
        }

        public void Add(INotification item)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (item is not T casted)
            {
                throw new InvalidCastException();
            }

            _items.Add(casted);
        }

        public async Task CommitAsync(CancellationToken cancellation)
        {
            await CommitCoreAsync(cancellation);
            foreach (var observer in _observers)
            {
                observer.Notify();
            }

            //TODO: Clear
        }

        protected abstract Task CommitCoreAsync(CancellationToken cancellation);

        public void SubscribeCommited(INotification state, Action<INotification> callback)
        {
            _observers.Add(new Observer(state, callback));
        }

        private class Observer
        {
            private readonly INotification _state;
            private readonly Action<INotification> _callback;

            public Observer(INotification state, Action<INotification> callback)
            {
                _state = state;
                _callback = callback;
            }

            public void Notify()
            {
                _callback.Invoke(_state);
            }
        }
    }
}
