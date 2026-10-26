using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>
    /// شناسنامهٔ یک Provider؛ خودِ Provider مشخصات بازار و قابلیت‌هایش را اعلام می‌کند و سیستم
    /// از روی آن ردیف بازار را می‌سازد و jobها را زمان‌بندی می‌کند. برای بازار جدید enum یا seed لازم نیست.
    /// </summary>
    public sealed class ProviderDescriptor
    {
        /// <summary>کلید پایدار Provider (مثل nobitex)؛ در MarketSettings برای انتخاب منبع فعال استفاده می‌شود.</summary>
        public required string ProviderKey { get; init; }

        /// <summary>کلید بازاری که این Provider دادهٔ آن را می‌دهد (مثل crypto-toman).</summary>
        public required string MarketKey { get; init; }

        /// <summary>نام انگلیسی بازار برای نمایش.</summary>
        public required string MarketName { get; init; }

        /// <summary>نام فارسی بازار برای نمایش.</summary>
        public required string MarketPersianName { get; init; }

        /// <summary>واحد پول قیمت‌ها (مثلاً IRR، IRT، USDT).</summary>
        public required string CurrencyCode { get; init; }

        /// <summary>نوع تقویم معاملاتی بازار.</summary>
        public required TradingCalendarKind CalendarKind { get; init; }

        /// <summary>شناسهٔ منطقهٔ زمانی بازار (مثلاً Asia/Tehran).</summary>
        public required string TimeZoneId { get; init; }

        /// <summary>
        /// تایم‌فریم‌هایی که منبع خودش مستقیم می‌دهد. هفتگی و ماهانه فقط وقتی اینجا می‌آیند که منبع
        /// روزانه نداشته باشد (مثلاً داده ماهانهٔ مسکن)؛ در غیر این صورت از روی روزانه ساخته می‌شوند.
        /// </summary>
        public required IReadOnlyList<Timeframe> NativeTimeframes { get; init; }

        /// <summary>وضعیت تعدیل کندل‌هایی که منبع می‌دهد.</summary>
        public AdjustmentMode Adjustment { get; init; } = AdjustmentMode.NotApplicable;

        /// <summary>
        /// وقتی یک بازار برای یک قابلیت چند Provider دارد و اپراتور هنوز انتخاب نکرده، Bootstrap این Provider را پیش‌فرض انتخاب می‌کند.
        /// برای هر (بازار، قابلیت) حداکثر یک Provider باید true باشد.
        /// </summary>
        public bool IsDefault { get; init; }

        /// <summary>سقف تعداد درخواست در دقیقه به منبع؛ خالی یعنی محدودیت اعلام‌شده‌ای نیست.</summary>
        public int? MaxRequestsPerMinute { get; init; }
    }
}
