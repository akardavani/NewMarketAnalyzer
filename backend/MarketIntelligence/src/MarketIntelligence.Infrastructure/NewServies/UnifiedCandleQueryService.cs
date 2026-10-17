using MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketIntelligence.Infrastructure
{
    public class UnifiedCandleQueryService : IUnifiedCandleQueryService
    {
        private readonly IUnifiedCandleRepository _repo;

        public UnifiedCandleQueryService(IUnifiedCandleRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<UnifiedCandleRecord>> GetCandlesAsync(
            CandleQuery query,
            CancellationToken ct)
        {
            var q = _repo.Query();

            if (query.Markets is { Count: > 0 })
            {
                q = q.Where(x => query.Markets.Contains(x.MarketId));
            }

            if (query.SymbolKeys is { Count: > 0 })
            {
                q = q.Where(x => query.SymbolKeys.Contains(x.SymbolKey));
            }

            if (query.Timeframe.HasValue)
            {
                q = q.Where(x => x.TimeframeId == query.Timeframe.Value);
            }

            if (query.From.HasValue)
            {
                q = q.Where(x => x.Date >= query.From.Value);
            }

            if (query.To.HasValue)
            {
                q = q.Where(x => x.Date <= query.To.Value);
            }

            return await q
                //.Include(x => x.Market)
                //.Include(x => x.Timeframe)
                .OrderBy(x => x.MarketId)
                .ThenBy(x => x.SymbolKey)
                .ThenBy(x => x.TimeframeId)
                .ThenBy(x => x.Date)
                .ToListAsync(ct);
        }
    }
}


