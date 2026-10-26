using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// پنج سطح برتر صف خرید و فروش نمادهای بورس (BestLimitsAllIns).
    /// <para>
    /// <b>نگاشت فیلدها (اصلاح باگ کد قدیمی):</b> در کد قدیمی ستون «تعداد» با «قیمت» جابه‌جا نگاشت شده بود.
    /// خرید: قیمت = PMeDem، حجم = QTitMeDem، تعداد سفارش = ZOrdMeDem. فروش: قیمت = PMeOf، حجم = QTitMeOf، تعداد سفارش = ZOrdMeOf.
    /// </para>
    /// نمادهای فعالی که در پاسخ هیچ سطحی ندارند با Snapshot خالی گزارش می‌شوند تا صف قدیمی‌شان پاک شود.
    /// </summary>
    public sealed class TsetmcStockOrderBookProvider : IOrderBookProvider
    {
        private readonly TsetmcWebServiceClient _client;
        private readonly TseSymbolDirectory _directory;
        private readonly TimeProvider _time;
        private readonly ILogger<TsetmcStockOrderBookProvider> _logger;

        /// <summary>سازنده.</summary>
        public TsetmcStockOrderBookProvider(TsetmcWebServiceClient client, TseSymbolDirectory directory, TimeProvider time, ILogger<TsetmcStockOrderBookProvider> logger)
        {
            _client = client;
            _directory = directory;
            _time = time;
            _logger = logger;
        }

        /// <inheritdoc />
        public ProviderDescriptor Descriptor { get; } = TseMarket.Descriptor(TseMarket.TsetmcProviderKey);

        /// <inheritdoc />
        public async Task<FetchResult<IReadOnlyList<OrderBookSnapshot>>> FetchOrderBooksAsync(CancellationToken cancellationToken)
        {
            try
            {
                var map = await _directory.GetInsCodeMapAsync(cancellationToken);
                if (map.Count == 0)
                    return FetchResult<IReadOnlyList<OrderBookSnapshot>>.Failed("Stock catalog is empty; sync symbols before order-book ingestion.");

                var rows = await _client.GetBestLimitsAsync(cancellationToken);
                if (rows.Count == 0)
                    return FetchResult<IReadOnlyList<OrderBookSnapshot>>.SuccessEmpty();

                var now = _time.GetUtcNow().UtcDateTime;
                var levelsByKey = new Dictionary<string, List<OrderBookLevelInfo>>(StringComparer.Ordinal);

                foreach (var row in rows)
                {
                    var insCode = XmlValue.Str(row, "InsCode");
                    var level = XmlValue.Int(row, "number");
                    if (insCode == null || level == null || !map.TryGetValue(insCode, out var key))
                        continue;

                    if (!levelsByKey.TryGetValue(key, out var list))
                        levelsByKey[key] = list = new List<OrderBookLevelInfo>();

                    AddLevel(list, OrderBookSide.Buy, level.Value,
                        XmlValue.Dec(row, "PMeDem"), XmlValue.Dec(row, "QTitMeDem"), XmlValue.Int(row, "ZOrdMeDem"));
                    AddLevel(list, OrderBookSide.Sell, level.Value,
                        XmlValue.Dec(row, "PMeOf"), XmlValue.Dec(row, "QTitMeOf"), XmlValue.Int(row, "ZOrdMeOf"));
                }

                // نمادی که الان هیچ سطحی ندارد → Snapshot خالی (حذف صف قدیمی).
                var snapshots = map.Values
                    .Select(key => new OrderBookSnapshot
                    {
                        SymbolKey = key,
                        SnapshotTimeUtc = now,
                        Levels = levelsByKey.TryGetValue(key, out var levels) ? levels : Array.Empty<OrderBookLevelInfo>()
                    })
                    .ToList();

                return FetchResult<IReadOnlyList<OrderBookSnapshot>>.Success(snapshots, snapshots.Count);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "TSETMC order book fetch failed");
                return FetchResult<IReadOnlyList<OrderBookSnapshot>>.Failed($"TSETMC order book fetch failed: {ex.Message}", ex);
            }
        }

        /// <summary>سطح خالی (قیمت و حجم صفر یا ناموجود) اضافه نمی‌شود.</summary>
        private static void AddLevel(List<OrderBookLevelInfo> list, OrderBookSide side, int level, decimal? price, decimal? volume, int? orderCount)
        {
            if (price is not > 0 || volume is not > 0)
                return;

            list.Add(new OrderBookLevelInfo
            {
                Side = side,
                LevelNumber = level,
                Price = price.Value,
                Volume = volume.Value,
                OrderCount = orderCount ?? 0
            });
        }
    }
}
