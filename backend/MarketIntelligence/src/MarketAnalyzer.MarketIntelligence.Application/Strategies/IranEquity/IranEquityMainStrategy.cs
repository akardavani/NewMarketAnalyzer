using MarketAnalyzer.MarketIntelligence.Application.Strategies.IranEquityMain;
using MarketAnalyzer.MarketIntelligence.Infrastructure;

namespace MarketAnalyzer.MarketIntelligence.Application
{
    /// <summary>Compatibility adapter for the generic strategy endpoint; business rules live in IranEquityMainHandler.</summary>
    public sealed class IranEquityMainStrategy : IAnalysisStrategy
    {
        private readonly IranEquityMainHandler _handler;

        public string Name => StrategyName.IranEquityMain;

        public IranEquityMainStrategy(IranEquityMainHandler handler) => _handler = handler;

        public async Task<AnalysisResult> ExecuteAsync(AnalysisRequest request, CancellationToken cancellationToken)
        {
            var result = await _handler.HandleAsync(new RunIranEquityMainQuery
            {
                InputFilePath = request.Parameters["InputFilePath"].ToString() ?? string.Empty
            }, cancellationToken);

            return new AnalysisResult { Strategy = Name, Data = result };
        }
    }
}
