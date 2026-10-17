using System.Threading.Tasks;
using System.Threading;

namespace MarketAnalyzer.Abstractions
{
    public interface INotificationHandler<in TNotification>
        where TNotification : INotification
    {
        /// <summary>
        /// Handles a notification
        /// </summary>
        /// <param name="notification">The notification</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task HandleAsync(TNotification notification, CancellationToken cancellationToken);
    }
}