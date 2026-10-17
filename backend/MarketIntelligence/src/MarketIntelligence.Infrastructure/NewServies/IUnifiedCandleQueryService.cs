using MarketIntelligence.Domain;

namespace MarketIntelligence.Infrastructure
{
    public interface IUnifiedCandleQueryService
    {
        Task<List<UnifiedCandleRecord>> GetCandlesAsync(
            CandleQuery query,
            CancellationToken ct);
    }
}


