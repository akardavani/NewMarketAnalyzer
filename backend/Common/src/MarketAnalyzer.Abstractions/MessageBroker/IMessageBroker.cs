using System;

namespace MarketAnalyzer.Abstractions
{
    public interface IMessageBroker : IAsyncDisposable
    {
        IProducer<T> CreateProducer<T>(string topic);
        IConsumer<T> CreateConsumer<T>(string topic);
    }
}
