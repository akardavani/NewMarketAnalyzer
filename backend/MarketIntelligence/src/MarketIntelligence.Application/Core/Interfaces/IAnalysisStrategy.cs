
namespace MarketIntelligence.Application
{
    public interface IAnalysisStrategy
    {
        string Name { get; }

        Task<AnalysisResult> ExecuteAsync(AnalysisRequest request, CancellationToken cancellation);
    }
}
