using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>کندل روزانهٔ شاخص‌های بورس تهران از نهایت‌نگر؛ شاخص تعدیل ندارد. واحد «امتیاز» است نه پول.</summary>
    public sealed class NahayatnegarIndexCandleProvider : IHistoricalCandleProvider
    {
        private readonly NahayatnegarCandleClient _client;

        /// <summary>سازنده.</summary>
        public NahayatnegarIndexCandleProvider(NahayatnegarCandleClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public ProviderDescriptor Descriptor { get; } = new()
        {
            ProviderKey = NahayatnegarCandleClient.ProviderKey,
            MarketKey = "tse-index",
            MarketName = "Tehran Stock Exchange Indices",
            MarketPersianName = "شاخص‌های بورس تهران",
            CurrencyCode = "PTS",
            CalendarKind = TradingCalendarKind.IranStockWeek,
            TimeZoneId = "Asia/Tehran",
            // فعلاً فقط روزانه؛ هفتگی و ماهانه از روی روزانه ساخته می‌شوند. زیرروزانه بعداً با افزودن مقدار به این فهرست.
            NativeTimeframes = new[] { Timeframe.Daily },
            Adjustment = AdjustmentMode.NotApplicable
        };

        /// <inheritdoc />
        public Task<FetchResult<CandleBatch>> FetchCandlesAsync(CandleRequest request, CancellationToken cancellationToken)
            => _client.FetchAsync(request, cancellationToken);
    }
}
