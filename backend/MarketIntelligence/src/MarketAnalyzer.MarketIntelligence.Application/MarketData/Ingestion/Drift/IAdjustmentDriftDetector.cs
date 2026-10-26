using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>
    /// تشخیص تغییر تعدیل منبع با مقایسهٔ پنجرهٔ هم‌پوشانی. وقتی منبع روی سری تعدیل‌شده کار می‌کند، افزایش سرمایه یا سود نقدی
    /// همهٔ قیمت‌های قبل از رویداد را با یک ضریب ثابت عوض می‌کند؛ این کلاس همین الگو را پیدا می‌کند.
    /// </summary>
    public interface IAdjustmentDriftDetector
    {
        /// <summary>کندل‌های ذخیره‌شده (صعودی) را با کندل‌های بستهٔ تازه مقایسه می‌کند.</summary>
        DriftAssessment Detect(IReadOnlyList<HistoricalCandle> stored, IReadOnlyList<CandleData> incomingClosed);
    }
}
