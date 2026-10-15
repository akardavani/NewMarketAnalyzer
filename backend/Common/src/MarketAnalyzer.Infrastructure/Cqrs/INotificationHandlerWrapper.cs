using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.Cqrs
{
    internal interface INotificationHandlerWrapper
    {
        Task HandleAsync(
            INotification notification,
            IServiceProvider serviceProvider,
            Func<IEnumerable<Func<INotification, CancellationToken, Task>>, INotification, CancellationToken, Task> publish,
            CancellationToken cancellationToken);
    }
}
