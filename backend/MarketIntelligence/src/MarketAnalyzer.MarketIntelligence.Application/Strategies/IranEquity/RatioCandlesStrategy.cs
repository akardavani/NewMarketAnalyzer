using MarketAnalyzer.MarketIntelligence.Application.Strategies.RatioCandles;
using MarketAnalyzer.MarketIntelligence.Infrastructure;
using MarketAnalyzer.SharedKernel;

namespace MarketAnalyzer.MarketIntelligence.Application
{
    /// <summary>Compatibility adapter for the generic strategy endpoint; the business rules live in RunRatioCandlesHandler.</summary>
    public sealed class RatioCandlesStrategy : IAnalysisStrategy
    {
        private const string SymbolParameter = "Symbols";
        private const string StartDateParameter = "StartDate";
        private const string EndDateParameter = "EndDate";
        private readonly RunRatioCandlesHandler _handler;

        public string Name => StrategyName.RatioCandles;

        public RatioCandlesStrategy(RunRatioCandlesHandler handler) => _handler = handler;

        public async Task<AnalysisResult> ExecuteAsync(AnalysisRequest request, CancellationToken cancellation)
        {
            var result = await _handler.HandleAsync(new RunRatioCandlesQuery
            {
                Symbols = ParameterHelper.GetRequiredStringListParameter(request, SymbolParameter),
                StartDate = ParameterHelper.GetRequiredParameter<DateTime>(request, StartDateParameter),
                EndDate = ParameterHelper.GetRequiredParameter<DateTime>(request, EndDateParameter)
            }, cancellation);

            return new AnalysisResult { Strategy = Name, Data = result };
        }
    }
}
