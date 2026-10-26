using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// مشخصات بازار سهام بورس تهران که همهٔ Providerهای این بازار باید یکسان اعلام کنند
    /// (ردیف بازار از روی همین مقدارها ساخته می‌شود؛ اختلاف بین Providerها باعث جابه‌جایی بی‌دلیل ردیف می‌شود).
    /// </summary>
    internal static class TseMarket
    {
        /// <summary>کلید بازار.</summary>
        public const string Key = "tse";

        /// <summary>کلید Provider سرویس‌های TSETMC.</summary>
        public const string TsetmcProviderKey = "tsetmc";

        /// <summary>منطقهٔ زمانی بازار.</summary>
        public const string TimeZoneId = "Asia/Tehran";

        /// <summary>شناسنامهٔ Provider بر پایهٔ مشخصات بازار.</summary>
        public static ProviderDescriptor Descriptor(string providerKey, AdjustmentMode adjustment = AdjustmentMode.NotApplicable) => new()
        {
            ProviderKey = providerKey,
            MarketKey = Key,
            MarketName = "Tehran Stock Exchange",
            MarketPersianName = "بورس تهران",
            CurrencyCode = "IRR",
            CalendarKind = TradingCalendarKind.IranStockWeek,
            TimeZoneId = TimeZoneId,
            NativeTimeframes = new[] { Timeframe.Daily },
            Adjustment = adjustment
        };
    }
}
