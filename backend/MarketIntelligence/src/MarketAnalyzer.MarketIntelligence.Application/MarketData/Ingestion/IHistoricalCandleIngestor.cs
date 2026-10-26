using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>
    /// ingestion کندل تاریخی یک نماد: دریافت از Provider، اعتبارسنجی، تشخیص تغییر تعدیل، ذخیره و ساخت هفتگی/ماهانه.
    /// Providerها فقط داده می‌دهند؛ تصمیم دربارهٔ ذخیره و جایگزینی فقط اینجاست.
    /// </summary>
    public interface IHistoricalCandleIngestor
    {
        /// <summary>
        /// یک نماد را به‌روز می‌کند. هرگز استثنا (جز لغو) نمی‌اندازد و شکست را در نتیجه گزارش می‌کند.
        /// هر نماد باید در هر لحظه فقط توسط یک نویسنده پردازش شود (job غیرهمپوشان).
        /// </summary>
        Task<SymbolIngestionResult> IngestAsync(IHistoricalCandleProvider provider, MarketSymbol symbol, CancellationToken cancellationToken);
    }
}
