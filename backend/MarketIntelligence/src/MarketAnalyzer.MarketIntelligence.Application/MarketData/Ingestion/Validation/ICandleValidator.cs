using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>اعتبارسنجی و نرمال‌سازی کندل‌های خام Provider پیش از هر ذخیره.</summary>
    public interface ICandleValidator
    {
        /// <summary>
        /// زمان‌ها را UTC و روی مرز بازه می‌کند، تکراری‌ها را یکتا می‌کند (آخری برنده)، کندل نامعتبر را رد می‌کند،
        /// High/Low ناسازگار را اصلاح می‌کند و کندل ناتمام را از کندل‌های بسته جدا می‌کند.
        /// </summary>
        ValidatedCandles Validate(CandleBatch batch, ITradingCalendar calendar, DateTime nowUtc);
    }
}
