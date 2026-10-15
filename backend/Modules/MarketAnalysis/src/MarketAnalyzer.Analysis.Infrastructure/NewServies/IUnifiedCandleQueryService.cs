using MarketAnalyzer.Domain;

namespace MarketAnalyzer.Analysis.Infrastructure
{
    public interface IUnifiedCandleQueryService
    {
        Task<List<UnifiedCandleRecord>> GetCandlesAsync(
            CandleQuery query,
            CancellationToken ct);
    }
}


