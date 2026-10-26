using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>
    /// محاسبهٔ ضریب تعدیل یک رویداد شرکتی برای مسیر Raw. فرمول دقیق افزایش سرمایه و سود در بازار ایران
    /// در این سند تعریف نشده است؛ پیاده‌سازی بعد از مقایسه با خروجی تعدیل‌شدهٔ فعلی نوشته می‌شود.
    /// </summary>
    public interface IAdjustmentFactorCalculator
    {
        /// <summary>کلید بازاری که این محاسبه‌گر برای آن است.</summary>
        string MarketKey { get; }

        /// <summary>ضریب تعدیل رویداد را حساب می‌کند. قیمت‌های قبل از تاریخ اجرا در این ضریب ضرب می‌شوند.</summary>
        decimal Calculate(CorporateActionInfo action, decimal closePriceBeforeEvent);
    }
}
