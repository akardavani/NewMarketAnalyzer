using MarketAnalyzer.MarketData.Application.Reading;

namespace MarketAnalyzer.MarketIntelligence.Application.Ports;

public interface IAnalysisCandleReader
{
    Task<IReadOnlyList<AnalysisCandle>> GetCandlesAsync(
        MultiSymbolCandleQuery query,
        CancellationToken cancellationToken);
}
