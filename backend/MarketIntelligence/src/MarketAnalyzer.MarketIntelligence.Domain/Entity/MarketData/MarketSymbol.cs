namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>
    /// کاتالوگ نمادها؛ هویت یک نماد در یک بازار را نگه می‌دارد، نه قیمت یا وضعیت لحظه‌ای را.
    /// کلیدهای یکتا: (MarketId, SymbolKey).
    /// </summary>
    public class MarketSymbol
    {
        /// <summary>شناسهٔ عددی نماد؛ جدول‌های کندل و وضعیت جاری با همین شناسه به نماد اشاره می‌کنند.</summary>
        public int Id { get; set; }

        /// <summary>شناسهٔ بازاری که نماد به آن تعلق دارد.</summary>
        public int MarketId { get; set; }

        /// <summary>
        /// کلید استاندارد نماد: بدون فاصله‌های اضافه، با یکسان‌سازی حروف و یونیکد.
        /// قاعدهٔ دقیق هر بازار را Provider همان بازار اعمال می‌کند.
        /// </summary>
        public string SymbolKey { get; set; } = null!;

        /// <summary>شناسهٔ نماد در منبع بیرونی (مثلاً InsCode در TSETMC). ممکن است خالی باشد.</summary>
        public string? ExternalId { get; set; }

        /// <summary>
        /// واحد پول قیمت‌های این نماد (مثلاً IRR، IRT، USD، USDT) اگر با واحد پیش‌فرض بازار فرق دارد؛ خالی یعنی همان واحد بازار.
        /// مثال: در بازار کریپتو، جفت‌ارز ETH-BTC قیمتش به BTC است نه USDT.
        /// </summary>
        public string? CurrencyCode { get; set; }

        /// <summary>نام انگلیسی یا لاتین نماد.</summary>
        public string? Name { get; set; }

        /// <summary>نام فارسی نماد.</summary>
        public string? PersianName { get; set; }

        /// <summary>آیا نماد فعال است؟ غیرفعال‌سازی فقط با دریافت کامل و موفق کاتالوگ انجام می‌شود.</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>اطلاعات اضافهٔ مختص بازار به‌صورت JSON؛ ساختار آن با MetadataVersion نسخه‌دار است.</summary>
        public string? MetadataJson { get; set; }

        /// <summary>نسخهٔ ساختار MetadataJson.</summary>
        public int MetadataVersion { get; set; }

        /// <summary>زمان ایجاد رکورد (UTC).</summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>زمان آخرین تغییر رکورد (UTC).</summary>
        public DateTime UpdatedAtUtc { get; set; }

        /// <summary>بازار نماد.</summary>
        public MarketDefinition Market { get; set; } = null!;
    }
}
