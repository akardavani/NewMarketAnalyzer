using System;

namespace MarketAnalyzer.Abstractions
{
    public interface IDomainEvent
    {
        public bool Batch { get; }
        public Func<INotification> Factory { get; }
        public Action<INotification> AfterPublish { get; }

        IBatchService ResolveBatchService(IServiceProvider serviceProvider);
    }
}