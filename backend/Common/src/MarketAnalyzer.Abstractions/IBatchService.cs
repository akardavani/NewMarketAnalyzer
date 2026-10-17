using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface IBatchService<T> : IBatchService where T : INotification
    {
        void Add(T item);
    }

    public interface IBatchService
    {
        void Add(INotification item);
        Task CommitAsync(CancellationToken cancellation);
        void SubscribeCommited(INotification state, Action<INotification> callback);
    }
}
