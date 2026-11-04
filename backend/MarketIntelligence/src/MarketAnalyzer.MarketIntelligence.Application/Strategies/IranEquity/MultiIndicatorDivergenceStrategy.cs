using MarketAnalyzer.MarketIntelligence.Application.Strategies.MultiIndicatorDivergence;
using MarketAnalyzer.MarketIntelligence.Infrastructure;

namespace MarketAnalyzer.MarketIntelligence.Application
{
    /// <summary>Compatibility adapter for the generic strategy endpoint; business rules live in MultiIndicatorDivergenceHandler.</summary>
    public sealed class MultiIndicatorDivergenceStrategy : IAnalysisStrategy
    {
        private readonly MultiIndicatorDivergenceHandler _handler;

        public string Name => StrategyName.MultiIndicatorDivergence;

        public MultiIndicatorDivergenceStrategy(MultiIndicatorDivergenceHandler handler) => _handler = handler;

        public async Task<AnalysisResult> ExecuteAsync(AnalysisRequest request, CancellationToken cancellationToken)
        {
            var result = await _handler.HandleAsync(new RunMultiIndicatorDivergenceQuery
            {
                InputFilePath = request.Parameters["InputFilePath"].ToString() ?? string.Empty
            }, cancellationToken);

            return new AnalysisResult { Strategy = Name, Data = result };
        }
    }
}
