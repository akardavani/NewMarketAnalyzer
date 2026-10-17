using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.Cqrs
{
    internal class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;
        private static readonly ConcurrentDictionary<Type, object> _requestHandlers = new();
        private static readonly ConcurrentDictionary<Type, INotificationHandlerWrapper> _notificationHandlers = new();

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellation = default)
        {
            var requestType = request.GetType();
            var handler = (IRequestHandlerWrapper<TResponse>)_requestHandlers.GetOrAdd(requestType,
                 t => Activator.CreateInstance(typeof(RequestHandlerWrapper<,>).MakeGenericType(requestType, typeof(TResponse))));

            return handler.HandleAsync(request, _serviceProvider, cancellation);
        }

        public Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            var notificationType = notification.GetType();
            var handler = _notificationHandlers.GetOrAdd(notificationType,
                t => (INotificationHandlerWrapper)Activator.CreateInstance(typeof(NotificationHandlerWrapper<>).MakeGenericType(notificationType)));

            return handler.HandleAsync(notification, _serviceProvider, PublishCore, cancellationToken);
        }

        private async Task PublishCore(
            IEnumerable<Func<INotification, CancellationToken, Task>> allHandlers,
            INotification notification,
            CancellationToken cancellationToken)
        {
            foreach (var handler in allHandlers)
            {
                await handler(notification, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
