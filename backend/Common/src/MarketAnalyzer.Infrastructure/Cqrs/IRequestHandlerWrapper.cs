using MarketAnalyzer.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.Cqrs
{
    internal interface IRequestHandlerWrapper<TResponse>
    {
        Task<TResponse> HandleAsync(IRequest<TResponse> request, IServiceProvider serviceProvider, CancellationToken cancellationToken);
    }
}
