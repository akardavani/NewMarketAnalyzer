using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface IMediator
    {
        Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellation = default);

        Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification;
    }
}