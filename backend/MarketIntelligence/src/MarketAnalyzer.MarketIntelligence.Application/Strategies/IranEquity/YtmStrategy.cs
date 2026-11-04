using MarketAnalyzer.MarketIntelligence.Application.Strategies.YtmFuture;
using MarketAnalyzer.MarketIntelligence.Infrastructure;

namespace MarketAnalyzer.MarketIntelligence.Application
{
    /// <summary>Compatibility adapter for the generic strategy endpoint; business rules live in YtmFutureHandler.</summary>
    public sealed class YtmStrategy : IAnalysisStrategy
    {
        private readonly YtmFutureHandler _handler;

        public string Name => StrategyName.YtmFuture;

        public YtmStrategy(YtmFutureHandler handler) => _handler = handler;

        public async Task<AnalysisResult> ExecuteAsync(AnalysisRequest request, CancellationToken cancellationToken)
        {
            var result = await _handler.HandleAsync(new RunYtmFutureQuery
            {
                Symbol = ParameterHelper.GetRequiredParameter<string>(request, "Symbol"),
                Calculates = ParameterHelper.GetRequiredParameter<string>(request, "Calculates"),
                StartDate = ParameterHelper.GetRequiredParameter<DateTime>(request, "StartDate"),
                EndDate = ParameterHelper.GetRequiredParameter<DateTime>(request, "EndDate")
            }, cancellationToken);

            return new AnalysisResult { Strategy = Name, Data = result };
        }
    }
}
