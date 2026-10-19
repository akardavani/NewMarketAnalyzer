using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public class UnifiedSymbolQueryService : IUnifiedSymbolQueryService
    {
        private readonly IUnifiedSymbolRepository _repo;

        public UnifiedSymbolQueryService(IUnifiedSymbolRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<UnifiedSymbolRecord>> GetSymbolsAsync(
            SymbolQuery query,
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

            if (query.Symbols is { Count: > 0 })
            {
                q = q.Where(x => x.Symbol != null && query.Symbols.Contains(x.Symbol));
            }

            if (query.InstrumentIds is { Count: > 0 })
            {
                q = q.Where(x => x.InstrumentId != null && query.InstrumentIds.Contains(x.InstrumentId));
            }

            if (query.InsCodes is { Count: > 0 })
            {
                q = q.Where(x => x.InsCode != null && query.InsCodes.Contains(x.InsCode));
            }

            if (query.IsActive.HasValue)
            {
                q = q.Where(x => x.IsActive == query.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.SearchText))
            {
                var text = query.SearchText.Trim();

                q = q.Where(x =>
                    (x.Symbol != null && x.Symbol.Contains(text)) ||
                    (x.SymbolKey != null && x.SymbolKey.Contains(text)) ||
                    (x.Name != null && x.Name.Contains(text)) ||
                    (x.PersianName != null && x.PersianName.Contains(text)) ||
                    (x.InstrumentId != null && x.InstrumentId.Contains(text)) ||
                    (x.InsCode != null && x.InsCode.Contains(text)));
            }

            return await q
                .OrderBy(x => x.MarketId)
                .ThenBy(x => x.SymbolKey)
                .ToListAsync(ct);
        }
    }
}


