using MarketAnalyzer.Abstractions;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{
    /// <summary>پیاده‌سازی EF برای <see cref="IMarketSymbolStore"/>.</summary>
    public sealed class EfMarketSymbolStore : IMarketSymbolStore
    {
        private readonly IDbContext _db;
        private readonly TimeProvider _time;

        /// <summary>سازنده.</summary>
        public EfMarketSymbolStore(IDbContext db, TimeProvider time)
        {
            _db = db;
            _time = time;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<MarketSymbol>> GetByMarketAsync(int marketId, bool onlyActive, CancellationToken cancellationToken)
        {
            var query = _db.Set<MarketSymbol>().AsNoTracking().Where(x => x.MarketId == marketId);
            if (onlyActive)
                query = query.Where(x => x.IsActive);
            return await query.OrderBy(x => x.SymbolKey).ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<SymbolSyncResult> SyncAsync(int marketId, IReadOnlyCollection<MarketSymbol> symbols, bool deactivateMissing, CancellationToken cancellationToken)
        {
            for (var attempt = 0; attempt < 2; attempt++)
            {
                var now = _time.GetUtcNow().UtcDateTime;

                // یکتاسازی ورودی بر اساس SymbolKey بدون حساسیت به حروف، هم‌راستا با collation پیش‌فرض SQL Server
                // که ایندکس یکتای (MarketId, SymbolKey) را هم بدون حساسیت به حروف می‌سنجد.
                var incoming = symbols
                    .Where(x => !string.IsNullOrWhiteSpace(x.SymbolKey))
                    .GroupBy(x => x.SymbolKey, StringComparer.OrdinalIgnoreCase)
                    .Select(g => g.Last())
                    .ToList();

                var existing = await _db.Set<MarketSymbol>()
                    .Where(x => x.MarketId == marketId)
                    .ToDictionaryAsync(x => x.SymbolKey, StringComparer.OrdinalIgnoreCase, cancellationToken);

                int inserted = 0, updated = 0, deactivated = 0;

                foreach (var item in incoming)
                {
                    if (!existing.TryGetValue(item.SymbolKey, out var current))
                    {
                        item.MarketId = marketId;
                        item.CreatedAtUtc = now;
                        item.UpdatedAtUtc = now;
                        _db.Set<MarketSymbol>().Add(item);
                        inserted++;
                        continue;
                    }

                    var changed = current.ExternalId != item.ExternalId
                                  || current.CurrencyCode != item.CurrencyCode
                                  || current.Name != item.Name
                                  || current.PersianName != item.PersianName
                                  || current.MetadataJson != item.MetadataJson
                                  || current.MetadataVersion != item.MetadataVersion
                                  || current.IsActive != item.IsActive;
                    if (!changed)
                        continue;

                    current.ExternalId = item.ExternalId;
                    current.CurrencyCode = item.CurrencyCode;
                    current.Name = item.Name;
                    current.PersianName = item.PersianName;
                    current.MetadataJson = item.MetadataJson;
                    current.MetadataVersion = item.MetadataVersion;
                    current.IsActive = item.IsActive;
                    current.UpdatedAtUtc = now;
                    updated++;
                }

                if (deactivateMissing)
                {
                    var incomingKeys = incoming.Select(x => x.SymbolKey).ToHashSet(StringComparer.OrdinalIgnoreCase);
                    foreach (var current in existing.Values)
                    {
                        if (current.IsActive && !incomingKeys.Contains(current.SymbolKey))
                        {
                            // حذف نمی‌کنیم: کندل‌ها و رویدادهای تاریخی به این نماد وصل‌اند.
                            current.IsActive = false;
                            current.UpdatedAtUtc = now;
                            deactivated++;
                        }
                    }
                }

                try
                {
                    await _db.SaveChangesAsync(cancellationToken);
                    return new SymbolSyncResult(inserted, updated, deactivated);
                }
                catch (DbUpdateException ex) when (attempt == 0 && UniqueViolation.IsUniqueViolation(ex))
                {
                    _db.ClearChangeTracker();
                }
            }

            throw new InvalidOperationException("Unreachable: SyncAsync retry loop exited without result.");
        }
    }
}
