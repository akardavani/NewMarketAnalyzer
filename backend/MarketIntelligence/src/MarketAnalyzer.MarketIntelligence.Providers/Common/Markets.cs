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
        /// شاخص‌های بورس و فرابورس (شاخص کل، هم‌وزن، گروه‌های صنعت). واحد «امتیاز» (PTS) است و تبدیل ارزی ندارد.
        /// فقط روزانه؛ هفتگی و ماهانه از روی روزانه ساخته می‌شوند.
        /// </summary>
        public static ProviderDescriptor TseIndex(string providerKey, bool isDefault = false) => new()
        {
            ProviderKey = providerKey,
            MarketKey = "tse-index",
            MarketName = "Tehran Stock Exchange Indices",
            MarketPersianName = "شاخص‌های بورس تهران",
            CurrencyCode = "PTS",
            CalendarKind = TradingCalendarKind.IranStockWeek,
            TimeZoneId = "Asia/Tehran",
            NativeTimeframes = new[] { Timeframe.Daily },
            Adjustment = AdjustmentMode.NotApplicable,
            IsDefault = isDefault
        };

        /// <summary>بورس کالای ایران (قراردادهای آتی). قیمت‌ها به ریال.</summary>
        public static ProviderDescriptor Ime(string providerKey, bool isDefault = false, int? maxRequestsPerMinute = null) => new()
        {
            ProviderKey = providerKey,
            MarketKey = "ime",
            MarketName = "Iran Mercantile Exchange (Futures)",
            MarketPersianName = "بورس کالا (آتی)",
            CurrencyCode = "IRR",
            CalendarKind = TradingCalendarKind.IranStockWeek,
            TimeZoneId = "Asia/Tehran",
            NativeTimeframes = new[] { Timeframe.Daily },
            Adjustment = AdjustmentMode.NotApplicable,
            IsDefault = isDefault,
            MaxRequestsPerMinute = maxRequestsPerMinute
        };

        /// <summary>
        /// قیمت مسکن (متوسط قیمت هر متر مربع منطقه‌ها از Kilid). داده <b>ماهانهٔ بومی</b> است و هر ماه یک عدد؛
        /// باز، بالا، پایین و پایانی هر دو برابر همان عدد است. واحد پول طبق فرض تومان است (باید با خروجی واقعی تطبیق داده شود).
        /// </summary>
        public static ProviderDescriptor RealEstate(string providerKey, bool isDefault = false, int? maxRequestsPerMinute = null) => new()
        {
            ProviderKey = providerKey,
            MarketKey = "real-estate",
            MarketName = "Real Estate (Tehran regions)",
            MarketPersianName = "مسکن (مناطق تهران)",
            CurrencyCode = "IRT",
            CalendarKind = TradingCalendarKind.Continuous24x7,
            TimeZoneId = "Asia/Tehran",
            NativeTimeframes = new[] { Timeframe.Mountly },
            Adjustment = AdjustmentMode.NotApplicable,
            IsDefault = isDefault,
            MaxRequestsPerMinute = maxRequestsPerMinute,
            // ۸۴۷ منطقه × یک درخواست؛ داده ماهی یک‌بار عوض می‌شود، پس هفته‌ای یک‌بار کافی است.
            CandleRefreshInterval = TimeSpan.FromDays(7)
        };

        /// <summary>صندوق‌های سرمایه‌گذاری (فهرست و NAV از فیپیران). قیمت واحد (NAV) به ریال.</summary>
        public static ProviderDescriptor Fund(string providerKey, bool isDefault = false) => new()
        {
            ProviderKey = providerKey,
            MarketKey = "fund",
            MarketName = "Investment Funds",
            MarketPersianName = "صندوق‌های سرمایه‌گذاری",
            CurrencyCode = "IRR",
            CalendarKind = TradingCalendarKind.IranStockWeek,
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
