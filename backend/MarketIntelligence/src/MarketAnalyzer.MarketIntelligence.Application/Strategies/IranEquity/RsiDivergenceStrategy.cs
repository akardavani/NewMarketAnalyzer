using MarketAnalyzer.MarketIntelligence.Application.Strategies.RsiDivergence;
using MarketAnalyzer.MarketIntelligence.Infrastructure;

namespace MarketAnalyzer.MarketIntelligence.Application;

/// <summary>Compatibility adapter for the generic strategy endpoint. New clients use the typed action.</summary>
public sealed class RsiDivergenceStrategy : IAnalysisStrategy
{
    private readonly RsiDivergenceHandler _handler;

    public string Name => StrategyName.RsiDivergence;

    public RsiDivergenceStrategy(RsiDivergenceHandler handler) => _handler = handler;

    public async Task<AnalysisResult> ExecuteAsync(AnalysisRequest request, CancellationToken cancellationToken)
    {
        var result = await _handler.HandleAsync(new RunRsiDivergenceQuery
        {
            IncludeSuspects = !request.Parameters.TryGetValue("IncludeSuspects", out var suspects) || Convert.ToBoolean(suspects),
            IncludeBacktest = !request.Parameters.TryGetValue("IncludeBacktest", out var backtest) || Convert.ToBoolean(backtest)
        }, cancellationToken);
        return new AnalysisResult { Strategy = Name, Data = result };
    }
}
