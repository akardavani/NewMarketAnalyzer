using MarketAnalyzer.Abstractions;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{
    /// <summary>پیاده‌سازی EF برای <see cref="IMarketSettingStore"/>.</summary>
    public sealed class EfMarketSettingStore : IMarketSettingStore
    {
        private readonly IDbContext _db;
        private readonly TimeProvider _time;

        /// <summary>سازنده.</summary>
        public EfMarketSettingStore(IDbContext db, TimeProvider time)
        {
            _db = db;
            _time = time;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<MarketSetting>> GetByScopeAsync(string scope, CancellationToken cancellationToken)
        {
            return await _db.Set<MarketSetting>().AsNoTracking()
                .Where(x => x.Scope == scope)
                .OrderBy(x => x.Key)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<string> GetValueAsync(string scope, string key, CancellationToken cancellationToken)
        {
            return _db.Set<MarketSetting>().AsNoTracking()
                .Where(x => x.Scope == scope && x.Key == key)
                .Select(x => x.Value)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> InsertIfMissingAsync(string scope, string key, string value, CancellationToken cancellationToken)
        {
            var exists = await _db.Set<MarketSetting>().AnyAsync(x => x.Scope == scope && x.Key == key, cancellationToken);
            if (exists)
                return false;

            _db.Set<MarketSetting>().Add(new MarketSetting
            {
                Scope = scope,
                Key = key,
                Value = value,
                UpdatedAtUtc = _time.GetUtcNow().UtcDateTime,
                UpdatedBy = "bootstrap"
            });

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (DbUpdateException ex) when (UniqueViolation.IsUniqueViolation(ex))
            {
                // نمونهٔ دیگری (مثلاً اجرای همزمان Bootstrap) زودتر درج کرده؛ مقدارش را دست نمی‌زنیم.
                _db.ClearChangeTracker();
                return false;
            }
        }

        /// <inheritdoc />
        public async Task SetAsync(string scope, string key, string value, string updatedBy, CancellationToken cancellationToken)
        {
            for (var attempt = 0; attempt < 2; attempt++)
            {
                var now = _time.GetUtcNow().UtcDateTime;
                var current = await _db.Set<MarketSetting>().FirstOrDefaultAsync(x => x.Scope == scope && x.Key == key, cancellationToken);
                if (current == null)
                {
                    _db.Set<MarketSetting>().Add(new MarketSetting { Scope = scope, Key = key, Value = value, UpdatedAtUtc = now, UpdatedBy = updatedBy });
                }
                else
                {
                    current.Value = value;
                    current.UpdatedAtUtc = now;
                    current.UpdatedBy = updatedBy;
                }

                try
                {
                    await _db.SaveChangesAsync(cancellationToken);
                    _db.ClearChangeTracker();
                    return;
                }
                catch (DbUpdateException ex) when (attempt == 0 && UniqueViolation.IsUniqueViolation(ex))
                {
                    _db.ClearChangeTracker();
                }
            }
        }
    }
}
