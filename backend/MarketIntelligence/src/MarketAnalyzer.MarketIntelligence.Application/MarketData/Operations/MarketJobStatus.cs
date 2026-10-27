namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <summary>وضعیت پایانی یک اجرای کار.</summary>
    public enum MarketJobStatus
    {
        /// <summary>کار کامل شد (ممکن است بعضی نمادها خطا داشته باشند؛ شمارندهٔ Failed را ببین).</summary>
        Completed = 1,

        /// <summary>کار اجرا نشد: Provider انتخاب نشده، بازار غیرفعال، یا اجرای قبلی هنوز در جریان است. خطا حساب نمی‌شود.</summary>
        Skipped = 2,

        /// <summary>کار شکست خورد (منبع در دسترس نبود یا خروجی نامعتبر بود). هیچ دادهٔ موجودی حذف نشده است.</summary>
        Failed = 3
    }
}
