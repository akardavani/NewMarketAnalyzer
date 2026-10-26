using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// مشخصات بازارهای غیر از بورس که همهٔ Providerهای هر بازار باید یکسان اعلام کنند
    /// (ردیف بازار از روی همین مقدارها ساخته می‌شود). بورس در <c>TseMarket</c> است.
    /// </summary>
    internal static class Markets
    {
        /// <summary>کریپتوی دلاری (KuCoin). قیمت‌ها پیش‌فرض به USDT است؛ نماد با واحد دیگر (مثل BTC) خودش CurrencyCode دارد.</summary>
        public static ProviderDescriptor Crypto(string providerKey, bool isDefault = false) => new()
        {
            ProviderKey = providerKey,
            MarketKey = "crypto",
            MarketName = "Crypto (USDT)",
            MarketPersianName = "کریپتو",
            CurrencyCode = "USDT",
            CalendarKind = TradingCalendarKind.Continuous24x7,
            TimeZoneId = "UTC",
            NativeTimeframes = new[] { Timeframe.Daily },
            Adjustment = AdjustmentMode.NotApplicable,
            IsDefault = isDefault
        };

        /// <summary>
        /// کریپتو به تومان (Wallex و Nobitex قابل‌تعویض). روز بازار به وقت تهران است. نماد با واحد دیگر
        /// (مثل USOON-USDT) خودش CurrencyCode دارد.
        /// </summary>
        public static ProviderDescriptor CryptoToman(string providerKey, bool isDefault = false) => new()
        {
            ProviderKey = providerKey,
            MarketKey = "crypto-toman",
            MarketName = "Crypto (Toman)",
            MarketPersianName = "کریپتو - تومان",
            CurrencyCode = "IRT",
            CalendarKind = TradingCalendarKind.Continuous24x7,
            TimeZoneId = "Asia/Tehran",
            NativeTimeframes = new[] { Timeframe.Daily },
            Adjustment = AdjustmentMode.NotApplicable,
            IsDefault = isDefault
        };

        /// <summary>
        /// بازار کلان (دلار، سکه، طلا، نقره، انس و حباب‌ها) از Tgju. واحد پیش‌فرض ریال است؛ انس جهانی دلار (USD)
        /// و حباب‌ها درصد (PCT، واحد غیرپولی) هستند و در خود نماد مشخص شده‌اند.
        /// </summary>
        public static ProviderDescriptor Macro(string providerKey, bool isDefault = false) => new()
        {
            ProviderKey = providerKey,
            MarketKey = "macro",
            MarketName = "Macro (Gold, Currency)",
            MarketPersianName = "بازار کلان (طلا و ارز)",
            CurrencyCode = "IRR",
            CalendarKind = TradingCalendarKind.Continuous24x7,
            TimeZoneId = "Asia/Tehran",
            NativeTimeframes = new[] { Timeframe.Daily },
            Adjustment = AdjustmentMode.NotApplicable,
            IsDefault = isDefault
        };
    }
}
