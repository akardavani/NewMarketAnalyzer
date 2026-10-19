using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public interface IUnifiedCandleQueryService
    {
        Task<List<UnifiedCandleRecord>> GetCandlesAsync(
            CandleQuery query,
            CancellationToken ct);
    }
}


