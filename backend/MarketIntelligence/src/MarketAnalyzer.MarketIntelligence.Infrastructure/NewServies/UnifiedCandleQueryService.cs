using MarketAnalyzer.Abstractions;
using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    /// <summary>
    /// سازگارکنندهٔ موقت برای مصرف‌کننده‌های قدیمی MarketIntelligence.
    /// قرارداد UnifiedCandleRecord را نگه می‌دارد، اما داده را از HistoricalCandles می‌خواند.
    /// </summary>
    public sealed class UnifiedCandleQueryService : IUnifiedCandleQueryService
    {
        private static readonly IReadOnlyDictionary<MarketType, string> MarketKeys = new Dictionary<MarketType, string>
        {
            [MarketType.Tse] = "tse",
            [MarketType.Macro] = "macro",
            [MarketType.Crypto] = "crypto",
            [MarketType.Ime] = "ime",
            [MarketType.InvestmentFund] = "fund",
            [MarketType.RealEstate] = "real-estate",
            [MarketType.TseIndex] = "tse-index",
            [MarketType.CryptoToman] = "crypto-toman"
        };

        private readonly IDbContext _db;
        private readonly IMarketDefinitionStore _markets;

        public UnifiedCandleQueryService(IDbContext db, IMarketDefinitionStore markets)
        {
            _db = db;
            _markets = markets;
        }

        public async Task<List<UnifiedCandleRecord>> GetCandlesAsync(CandleQuery query, CancellationToken ct)
        {
            var selectedMarkets = query.Markets is { Count: > 0 }
                ? query.Markets.Distinct().ToArray()
                : MarketKeys.Keys.ToArray();
            var definitions = await _markets.GetAllAsync(ct);
            var results = new List<UnifiedCandleRecord>();

            foreach (var marketType in selectedMarkets)
            {
                if (!MarketKeys.TryGetValue(marketType, out var marketKey))
                    continue;

                var market = definitions.FirstOrDefault(x => x.Key == marketKey);
                if (market is null)
                    continue;

                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(market.TimeZoneId);
                var candles = _db.Set<HistoricalCandle>().AsNoTracking()
                    .Where(x => x.Symbol.MarketId == market.Id);

                if (query.SymbolKeys is { Count: > 0 })
                    candles = candles.Where(x => query.SymbolKeys.Contains(x.Symbol.SymbolKey));
                if (query.Timeframe.HasValue)
                    candles = candles.Where(x => x.Timeframe == query.Timeframe.Value);
                if (query.From.HasValue)
                {
                    var fromUtc = ToUtcBound(query.From.Value, timeZone);
                    candles = candles.Where(x => x.OpenTimeUtc >= fromUtc);
                }
                if (query.To.HasValue)
                {
                    var toUtc = ToUtcBound(query.To.Value, timeZone);
                    candles = candles.Where(x => x.OpenTimeUtc <= toUtc);
                }

                var rows = await candles
                    .Select(x => new
                    {
                        x.Id,
                        x.Timeframe,
                        x.OpenTimeUtc,
                        x.Open,
                        x.High,
                        x.Low,
                        x.Close,
                        x.Volume,
                        x.Value,
                        x.ReceivedAtUtc,
                        x.Symbol.SymbolKey
                    })
                    .ToListAsync(ct);

                foreach (var row in rows)
                {
                    var localDate = TimeZoneInfo.ConvertTimeFromUtc(AsUtc(row.OpenTimeUtc), timeZone);
                    var record = new UnifiedCandleRecord
                    {
                        MarketId = marketType,
                        TimeframeId = row.Timeframe,
                        SymbolKey = row.SymbolKey,
                        Open = row.Open,
                        High = row.High,
                        Low = row.Low,
                        Close = row.Close,
                        Volume = row.Volume,
                        Value = row.Value,
                        Date = localDate,
                        PersianDate = localDate.ToPersian(),
                        CreatedAtUtc = row.ReceivedAtUtc
                    };
                    record.GenerateId();
                    results.Add(record);
                }
            }

            return results
                .OrderBy(x => x.MarketId)
                .ThenBy(x => x.SymbolKey)
                .ThenBy(x => x.TimeframeId)
                .ThenBy(x => x.Date)
                .ToList();
        }

        private static DateTime ToUtcBound(DateTime value, TimeZoneInfo timeZone)
        {
            if (value.Kind == DateTimeKind.Utc)
                return value;

            return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(value, DateTimeKind.Unspecified), timeZone);
        }

        private static DateTime AsUtc(DateTime value)
            => value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
}
