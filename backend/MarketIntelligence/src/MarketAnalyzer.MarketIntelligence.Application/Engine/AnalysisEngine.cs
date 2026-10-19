namespace MarketAnalyzer.MarketIntelligence.Application
{
    public class AnalysisEngine : IAnalysisEngine
    {
        private readonly AnalysisStrategyRegistry _registry;

        public AnalysisEngine(AnalysisStrategyRegistry registry)
        {
            _registry = registry;
        }

        public async Task<AnalysisResult> RunAsync(AnalysisRequest request, CancellationToken cancellation)
        {
            var strategy = _registry.Get(request.Strategy);

            return await strategy.ExecuteAsync(request, cancellation);
        }
    }
}
