using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>کندل روزانهٔ سهام و حق تقدم بورس تهران از نهایت‌نگر؛ سری تعدیل‌شده است (AlreadyAdjusted).</summary>
    public sealed class NahayatnegarStockCandleProvider : IHistoricalCandleProvider
    {
        private readonly NahayatnegarCandleClient _client;

        /// <summary>سازنده.</summary>
        public NahayatnegarStockCandleProvider(NahayatnegarCandleClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public ProviderDescriptor Descriptor { get; } = new()
        {
            ProviderKey = NahayatnegarCandleClient.ProviderKey,
            MarketKey = "tse",
            MarketName = "Tehran Stock Exchange",
            MarketPersianName = "بورس تهران",
            CurrencyCode = "IRR",
            CalendarKind = TradingCalendarKind.IranStockWeek,
            TimeZoneId = "Asia/Tehran",
            // فعلاً فقط روزانه؛ هفتگی و ماهانه از روی روزانه ساخته می‌شوند. زیرروزانه بعداً با افزودن مقدار به این فهرست.
            NativeTimeframes = new[] { Timeframe.Daily },
            Adjustment = AdjustmentMode.AlreadyAdjusted
        };

        /// <inheritdoc />
        public Task<FetchResult<CandleBatch>> FetchCandlesAsync(CandleRequest request, CancellationToken cancellationToken)
            => _client.FetchAsync(request, cancellationToken);
    }
}
