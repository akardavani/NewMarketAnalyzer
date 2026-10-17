using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface IRequestHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
    }

    public interface IRequestHandler<in TRequest> : IRequestHandler<TRequest, Void>
        where TRequest : IRequest<Void>
    {
    }
}