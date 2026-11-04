using MarketAnalyzer.MarketIntelligence.Application.Strategies.BottomReversal;
using MarketAnalyzer.MarketIntelligence.Infrastructure;

namespace MarketAnalyzer.MarketIntelligence.Application
{
    /// <summary>Compatibility adapter for the generic strategy endpoint; business rules live in BottomReversalHandler.</summary>
    public sealed class BottomReversalStrategy : IAnalysisStrategy
    {
        private readonly BottomReversalHandler _handler;

        public string Name => StrategyName.BottomReversal;

        public BottomReversalStrategy(BottomReversalHandler handler) => _handler = handler;

        public async Task<AnalysisResult> ExecuteAsync(AnalysisRequest request, CancellationToken cancellationToken)
        {
            var result = await _handler.HandleAsync(new RunBottomReversalQuery
            {
                InputFilePath = request.Parameters["InputFilePath"].ToString() ?? string.Empty,
                StartDate = request.Parameters.ContainsKey("StartDate")
                    ? ParameterHelper.GetRequiredParameter<DateTime>(request, "StartDate")
                    : DateTime.Today.AddDays(-30),
                EndDate = request.Parameters.ContainsKey("EndDate")
                    ? ParameterHelper.GetRequiredParameter<DateTime>(request, "EndDate")
                    : DateTime.Today
            }, cancellationToken);

            return new AnalysisResult { Strategy = Name, Data = result };
        }
    }
}
