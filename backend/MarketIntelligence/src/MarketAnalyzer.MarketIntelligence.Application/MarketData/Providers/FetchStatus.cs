namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>
    /// وضعیت نتیجهٔ یک دریافت از منبع بیرونی. تفاوت «موفق ولی خالی» و «شکست» اینجا صریح است
    /// تا شکست منبع هرگز به‌عنوان «داده‌ای وجود ندارد» تفسیر نشود و چیزی حذف نشود.
    /// </summary>
    public enum FetchStatus
    {
        /// <summary>دریافت کامل و موفق بود و داده دارد؛ جایگزینی داده قبلی مجاز است.</summary>
        Success = 1,

        /// <summary>منبع پاسخ معتبر داد و واقعاً داده‌ای نبود (مثلاً نماد هنوز کندل ندارد). چیزی حذف نمی‌شود.</summary>
        SuccessEmpty = 2,

        /// <summary>فقط بخشی از داده دریافت شد؛ بخش معتبر Upsert می‌شود ولی هیچ حذفی انجام نمی‌شود.</summary>
        Partial = 3,

        /// <summary>دریافت شکست خورد (خطای شبکه، پاسخ خراب، ...)؛ هیچ نوشتن یا حذفی انجام نمی‌شود.</summary>
        Failed = 4
    }
}
