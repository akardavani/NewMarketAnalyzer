using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface IOperationRequirementHandler<TRequirement> where TRequirement : class, IOperationRequirement
    {
        Task<RequirementEvaluationResult> EvaluateAsync(TRequirement requirement, CancellationToken cancellation);
    }
}
