using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>قابلیت دریافت کندل تاریخی یک نماد.</summary>
    public interface IHistoricalCandleProvider : IMarketDataProvider
    {
        /// <summary>کندل‌های یک نماد را بر اساس درخواست می‌دهد. کندل ناتمام (مثل امروز) را می‌تواند بدهد؛ جداسازی آن با اعتبارسنجی است.</summary>
        Task<FetchResult<CandleBatch>> FetchCandlesAsync(CandleRequest request, CancellationToken cancellationToken);
    }
}
