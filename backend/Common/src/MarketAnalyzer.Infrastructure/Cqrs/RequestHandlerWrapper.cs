using Microsoft.Extensions.DependencyInjection;
using MarketAnalyzer.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.Cqrs
{
    internal class RequestHandlerWrapper<TRequest, TResponse> : IRequestHandlerWrapper<TResponse>
        where TRequest : IRequest<TResponse>
    {
        public Task<TResponse> HandleAsync(IRequest<TResponse> request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
            return handler.HandleAsync((TRequest)request, cancellationToken);
        }
    }
}
