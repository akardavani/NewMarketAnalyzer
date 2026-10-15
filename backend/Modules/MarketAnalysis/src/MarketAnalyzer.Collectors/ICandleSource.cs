using MarketAnalyzer.Domain;

namespace MarketAnalyzer.Collectors
{
    public interface ICandleSource
    {
        MarketType MarketType { get; }
        SourceType SourceType { get; }

        Task<List<UnifiedCandleRecord>> FetchCandlesAsync(
            string symbol,
            Timeframe timeframe,
            int yearsAgo,
            CancellationToken cancellation);
    }
}
