using MarketAnalyzer.Domain;

namespace MarketAnalyzer.Analysis.Infrastructure
{
    public interface IUnifiedSymbolRepository
    {
        IQueryable<UnifiedSymbolRecord> Query();

        Task<UnifiedSymbolRecord?> GetAsync(
            MarketType marketId,
            string symbolKey,
            CancellationToken ct);

        Task<List<UnifiedSymbolRecord>> GetAllAsync(
            MarketType marketId,
            CancellationToken ct);

        Task UpsertManyAsync(
            List<UnifiedSymbolRecord> items,
            CancellationToken ct);

        Task DeleteAsync(
            MarketType marketId,
            string symbolKey,
            CancellationToken ct);
    }
}
