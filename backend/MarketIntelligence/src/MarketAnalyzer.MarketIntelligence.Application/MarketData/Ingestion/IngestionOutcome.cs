namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>پیامد ingestion یک نماد.</summary>
    public enum IngestionOutcome
    {
        /// <summary>دریافت دوره‌ای موفق بود و کندل‌ها Upsert شدند.</summary>
        Updated = 1,

        /// <summary>منبع پاسخ معتبر داد ولی کندل بسته‌ای نبود؛ چیزی تغییر نکرد.</summary>
        NoData = 2,

        /// <summary>کل تاریخچه جایگزین شد (اولین بار یا بعد از تشخیص تغییر تعدیل).</summary>
        FullyRefetched = 3,

        /// <summary>شکست خورد؛ هیچ دادهٔ ذخیره‌شده‌ای تغییر یا حذف نشد.</summary>
        Failed = 4
    }
}
