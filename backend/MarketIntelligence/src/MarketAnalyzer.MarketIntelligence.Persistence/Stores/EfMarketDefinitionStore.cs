using MarketAnalyzer.Abstractions;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{
    /// <summary>پیاده‌سازی EF برای <see cref="IMarketDefinitionStore"/>.</summary>
    public sealed class EfMarketDefinitionStore : IMarketDefinitionStore
    {
        private readonly IDbContext _db;
        private readonly TimeProvider _time;

        /// <summary>سازنده.</summary>
        public EfMarketDefinitionStore(IDbContext db, TimeProvider time)
        {
            _db = db;
            _time = time;
        }

        /// <inheritdoc />
        public async Task<MarketDefinition> EnsureAsync(MarketDefinition desired, CancellationToken cancellationToken)
        {
            // دو بار تلاش: اگر همزمان نویسندهٔ دیگری همین بازار را ساخت، دور دوم آن را موجود می‌بیند.
            for (var attempt = 0; attempt < 2; attempt++)
            {
                var now = _time.GetUtcNow().UtcDateTime;
                var existing = await _db.Set<MarketDefinition>().FirstOrDefaultAsync(x => x.Key == desired.Key, cancellationToken);

                if (existing == null)
                {
                    desired.CreatedAtUtc = now;
                    desired.UpdatedAtUtc = now;
                    _db.Set<MarketDefinition>().Add(desired);
                }
                else
                {
                    // IsActive عمداً دست نمی‌خورد: تصمیم اپراتور است، نه Provider.
                    existing.Name = desired.Name;
                    existing.PersianName = desired.PersianName;
                    existing.CurrencyCode = desired.CurrencyCode;
                    existing.CalendarKind = desired.CalendarKind;
                    existing.TimeZoneId = desired.TimeZoneId;
                    existing.UpdatedAtUtc = now;
                }

                try
                {
                    await _db.SaveChangesAsync(cancellationToken);
                    return existing ?? desired;
                }
                catch (DbUpdateException ex) when (attempt == 0 && UniqueViolation.IsUniqueViolation(ex))
                {
                    _db.ClearChangeTracker();
                }
            }

            throw new InvalidOperationException("Unreachable: EnsureAsync retry loop exited without result.");
        }

        /// <inheritdoc />
        public Task<MarketDefinition> GetByKeyAsync(string key, CancellationToken cancellationToken)
            => _db.Set<MarketDefinition>().AsNoTracking().FirstOrDefaultAsync(x => x.Key == key, cancellationToken);

        /// <inheritdoc />
        public async Task<IReadOnlyList<MarketDefinition>> GetAllAsync(CancellationToken cancellationToken)
            => await _db.Set<MarketDefinition>().AsNoTracking().OrderBy(x => x.Id).ToListAsync(cancellationToken);
    }
}
