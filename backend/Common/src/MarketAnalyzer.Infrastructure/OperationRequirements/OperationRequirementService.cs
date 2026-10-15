using Microsoft.Extensions.DependencyInjection;
using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.OperationRequirements
{
    internal class OperationRequirementService : IOperationRequirementService
    {
        private readonly IServiceProvider _serviceProvider;

        public OperationRequirementService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task RequireAsync<TRequirement>(TRequirement requirement, CancellationToken cancellation)
            where TRequirement : class, IOperationRequirement
        {
            using var scope = _serviceProvider.CreateScope();
            var handlers = scope.ServiceProvider.GetRequiredService<IEnumerable<IOperationRequirementHandler<TRequirement>>>();
            var errors = new List<string>();
            foreach (var handler in handlers)
            {
                var evaluationResult = await handler.EvaluateAsync(requirement, cancellation);
                if (!evaluationResult.Succeeded)
                {
                    errors.Add(evaluationResult.Error);
                }
            }

            if (errors.Count > 0)
            {
                throw new ClientExposedException(string.Join("\r\n", errors));
            }
        }

        public async Task<RequirementEvaluationResult> EvaluateAsync<TRequirement>(TRequirement requirement, CancellationToken cancellation)
            where TRequirement : class, IOperationRequirement
        {
            using var scope = _serviceProvider.CreateScope();
            var handlers = scope.ServiceProvider.GetRequiredService<IEnumerable<IOperationRequirementHandler<TRequirement>>>();
            foreach (var handler in handlers)
            {
                var evaluationResult = await handler.EvaluateAsync(requirement, cancellation);
                if (!evaluationResult.Succeeded)
                {
                    return evaluationResult;
                }
            }

            return RequirementEvaluationResult.Success();
        }
    }
}
