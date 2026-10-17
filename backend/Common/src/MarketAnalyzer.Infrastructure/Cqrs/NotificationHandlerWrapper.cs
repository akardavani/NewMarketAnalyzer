using Microsoft.Extensions.DependencyInjection;
using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.Cqrs
{
    internal class NotificationHandlerWrapper<TNotification> : INotificationHandlerWrapper
        where TNotification : INotification
    {
        public Task HandleAsync(
            INotification notification, 
            IServiceProvider serviceProvider,
            Func<IEnumerable<Func<INotification, CancellationToken, Task>>, INotification, CancellationToken, Task> publish,
            CancellationToken cancellationToken)
        {
            var handlers = serviceProvider
                .GetServices<INotificationHandler<TNotification>>()
                .Select(CreateCaller);

            return publish(handlers, notification, cancellationToken);
        }

        private static Func<INotification, CancellationToken, Task> CreateCaller(INotificationHandler<TNotification> handler)
        {
            return (notification, cancellationToken) => handler.HandleAsync((TNotification)notification, cancellationToken);
        }
    }
}
