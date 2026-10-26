using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>خروجی اعتبارسنجی کندل‌های دریافتی: کندل‌های بسته آمادهٔ ذخیره، کندل ناتمام و آمار رد/اصلاح.</summary>
    public sealed class ValidatedCandles
    {
        /// <summary>کندل‌های بستهٔ معتبر، یکتا و به ترتیب زمان صعودی. زمان‌ها UTC و روی مرز بازه‌اند.</summary>
        public required IReadOnlyList<CandleData> Closed { get; init; }

        /// <summary>جدیدترین کندل ناتمام (مثل امروز) اگر در دریافت بود؛ ذخیرهٔ تاریخی نمی‌شود.</summary>
        public CandleData? Open { get; init; }

        /// <summary>تعداد کندل‌های ردشده (قیمت غیرمثبت، حجم منفی، تاریخ آینده).</summary>
        public int RejectedCount { get; init; }

        /// <summary>تعداد کندل‌هایی که High/Low آن‌ها با Open/Close سازگار نبود و اصلاح شد.</summary>
        public int RepairedCount { get; init; }

        /// <summary>نمونه‌ای از مشکل‌های دیده‌شده برای لاگ (حداکثر ۲۰ مورد).</summary>
        public required IReadOnlyList<string> Issues { get; init; }
    }
}
