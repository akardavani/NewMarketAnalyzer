using MarketAnalyzer.Abstractions;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{
    /// <summary>پیاده‌سازی EF برای <see cref="ICorporateActionStore"/>.</summary>
    public sealed class EfCorporateActionStore : ICorporateActionStore
    {
        private readonly IDbContext _db;
        private readonly TimeProvider _time;

        /// <summary>سازنده.</summary>
        public EfCorporateActionStore(IDbContext db, TimeProvider time)
        {
            _db = db;
            _time = time;
        }

        /// <inheritdoc />
        public async Task<UpsertStats> UpsertAsync(int symbolId, IReadOnlyCollection<CorporateAction> actions, CancellationToken cancellationToken)
        {
            var incoming = actions
                .GroupBy(x => (x.ActionType, x.EffectiveDate))
                .Select(g => g.Last())
                .ToList();
            if (incoming.Count == 0)
                return new UpsertStats(0, 0);

            for (var attempt = 0; attempt < 2; attempt++)
            {
                var now = _time.GetUtcNow().UtcDateTime;
                var existing = (await _db.Set<CorporateAction>()
                        .Where(x => x.SymbolId == symbolId)
                        .ToListAsync(cancellationToken))
                    .ToDictionary(x => (x.ActionType, x.EffectiveDate));

                int inserted = 0, matched = 0;
                foreach (var item in incoming)
                {
                    if (existing.TryGetValue((item.ActionType, item.EffectiveDate), out var current))
                    {
                        current.IsDateApproximate = item.IsDateApproximate;
                        current.AdjustmentFactor = item.AdjustmentFactor;
                        current.Amount = item.Amount;
                        current.Origin = item.Origin;
                        current.Description = item.Description;
                        matched++;
                    }
                    else
                    {
                        _db.Set<CorporateAction>().Add(new CorporateAction
                        {
                            SymbolId = symbolId,
                            ActionType = item.ActionType,
                            EffectiveDate = item.EffectiveDate,
                            IsDateApproximate = item.IsDateApproximate,
                            AdjustmentFactor = item.AdjustmentFactor,
                            Amount = item.Amount,
                            Origin = item.Origin,
                            Description = item.Description,
                            CreatedAtUtc = now
                        });
                        inserted++;
                    }
                }

                try
                {
                    await _db.SaveChangesAsync(cancellationToken);
                    _db.ClearChangeTracker();
                    return new UpsertStats(inserted, matched);
                }
                catch (DbUpdateException ex) when (attempt == 0 && UniqueViolation.IsUniqueViolation(ex))
                {
                    _db.ClearChangeTracker();
                }
            }

            throw new InvalidOperationException("Unreachable: UpsertAsync retry loop exited without result.");
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<CorporateAction>> GetBySymbolAsync(int symbolId, CancellationToken cancellationToken)
        {
            return await _db.Set<CorporateAction>().AsNoTracking()
                .Where(x => x.SymbolId == symbolId)
                .OrderBy(x => x.EffectiveDate)
                .ToListAsync(cancellationToken);
        }
    }
}
