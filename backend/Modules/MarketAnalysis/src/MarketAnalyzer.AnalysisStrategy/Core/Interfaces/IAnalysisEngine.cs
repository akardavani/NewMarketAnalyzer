namespace MarketAnalyzer.AnalysisStrategy
{
    public interface IAnalysisEngine
    {
        Task<AnalysisResult> RunAsync(AnalysisRequest request, CancellationToken cancellation);
    }
}
