namespace MarketAnalyzer.MarketIntelligence.Application
{
    public interface IAnalysisEngine
    {
        Task<AnalysisResult> RunAsync(AnalysisRequest request, CancellationToken cancellation);
    }
}
