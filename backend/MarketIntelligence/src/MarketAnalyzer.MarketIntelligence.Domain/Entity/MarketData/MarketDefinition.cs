namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>
    /// تعریف یک بازار (مثلاً بورس تهران، ارز دیجیتال به تومان). این رکوردها از روی Descriptor
    /// Providerها هنگام بالا آمدن برنامه هم‌گام می‌شوند؛ برای بازار جدید نیازی به enum یا seed نیست.
    /// </summary>
    public class MarketDefinition
    {
        /// <summary>شناسهٔ عددی (Identity) که سایر جدول‌ها به آن اشاره می‌کنند.</summary>
        public int Id { get; set; }

        /// <summary>کلید پایدار انگلیسی بازار (مثل tse یا crypto-toman). یکتا و تغییرناپذیر است.</summary>
        public string Key { get; set; } = null!;

        /// <summary>نام انگلیسی نمایشی.</summary>
        public string Name { get; set; } = null!;

        /// <summary>نام فارسی نمایشی.</summary>
        public string PersianName { get; set; } = null!;

        /// <summary>واحد پول قیمت‌ها در این بازار (مثلاً IRR، IRT، USDT). برای مقایسهٔ بین بازارها لازم است.</summary>
        public string CurrencyCode { get; set; } = null!;

        /// <summary>نوع تقویم معاملاتی بازار.</summary>
        public TradingCalendarKind CalendarKind { get; set; }

        /// <summary>شناسهٔ منطقهٔ زمانی بازار (مثلاً Asia/Tehran) برای تعیین مرز روز.</summary>
        public string TimeZoneId { get; set; } = null!;

        /// <summary>آیا دریافت داده برای این بازار فعال است؟</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>زمان ایجاد رکورد (UTC).</summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>زمان آخرین تغییر رکورد (UTC).</summary>
        public DateTime UpdatedAtUtc { get; set; }
    }
}
