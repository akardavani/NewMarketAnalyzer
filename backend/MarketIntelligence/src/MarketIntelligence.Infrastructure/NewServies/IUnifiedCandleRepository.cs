using MarketIntelligence.Domain;

namespace MarketIntelligence.Infrastructure
{
    public interface IUnifiedCandleRepository
    {
        IQueryable<UnifiedCandleRecord> Query();
        Task BulkInsertAsync(List<UnifiedCandleRecord> items, CancellationToken ct);
        Task BulkDeleteAsync(List<string> ids, CancellationToken ct);
        Task BulkDeleteAsync(MarketType marketType, Timeframe timeframe, CancellationToken ct);
        Task UpsertManyAsync(IEnumerable<UnifiedCandleRecord> candles, CancellationToken cancellationToken);
        Task<List<UnifiedCandleRecord>> GetBySymbolAsync(MarketType marketId, string symbolKey, Timeframe timeframe,
            CancellationToken cancellationToken);
    }
}


