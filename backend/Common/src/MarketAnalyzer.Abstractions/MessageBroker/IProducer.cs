using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface IProducer<T> : IDisposable
    {
        Task PublishAsync(T message, CancellationToken cancellation = default);
    }
}
