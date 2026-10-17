using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface IOperationRequirementService
    {
        Task RequireAsync<TRequirement>(TRequirement requirement, CancellationToken cancellation)
            where TRequirement : class, IOperationRequirement;

        Task<RequirementEvaluationResult> EvaluateAsync<TRequirement>(TRequirement requirement, CancellationToken cancellation)
            where TRequirement : class, IOperationRequirement;
    }
}
