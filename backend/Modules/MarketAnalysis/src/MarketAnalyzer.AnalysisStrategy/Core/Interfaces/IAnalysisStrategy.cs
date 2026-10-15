
namespace MarketAnalyzer.AnalysisStrategy
{
    public interface IAnalysisStrategy
    {
        string Name { get; }

        Task<AnalysisResult> ExecuteAsync(AnalysisRequest request, CancellationToken cancellation);
    }
}
