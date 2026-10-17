using MarketIntelligence.Domain;

namespace MarketIntelligence.Collectors
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
