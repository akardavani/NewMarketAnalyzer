using MarketAnalyzer.MarketIntelligence.Application.Strategies.IranQueue;
using MarketAnalyzer.MarketIntelligence.Infrastructure;

namespace MarketAnalyzer.MarketIntelligence.Application
{
    /// <summary>Compatibility adapter for the generic strategy endpoint; business rules live in RunIranQueueHandler.</summary>
    public sealed class IranQueueStrategy : IAnalysisStrategy
    {
        private readonly RunIranQueueHandler _handler;

        public string Name => StrategyName.IranQueue;

        public IranQueueStrategy(RunIranQueueHandler handler) => _handler = handler;

        public async Task<AnalysisResult> ExecuteAsync(AnalysisRequest request, CancellationToken cancellationToken)
        {
            var result = await _handler.HandleAsync(new RunIranQueueQuery
            {
                InputFilePath = request.Parameters["InputFilePath"].ToString() ?? string.Empty,
                FromRatio = Convert.ToInt32(request.Parameters["FromRatio"].ToString()),
                ToRatio = Convert.ToInt32(request.Parameters["ToRatio"].ToString()),
                Return1Month = Convert.ToInt32(request.Parameters["Return1Month"].ToString())
            }, cancellationToken);

            return new AnalysisResult { Strategy = Name, Data = result };
        }
    }
}
