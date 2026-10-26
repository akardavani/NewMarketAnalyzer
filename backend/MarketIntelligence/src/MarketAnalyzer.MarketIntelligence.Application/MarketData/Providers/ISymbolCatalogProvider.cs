using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>قابلیت دریافت فهرست نمادهای بازار. فهرست‌های ثابت (مثل مناطق مسکن) هم با همین interface از منبع embedded می‌آیند.</summary>
    public interface ISymbolCatalogProvider : IMarketDataProvider
    {
        /// <summary>فهرست کامل نمادهای بازار را برمی‌گرداند. فقط نتیجهٔ Success کامل اجازهٔ غیرفعال‌سازی نمادهای حذف‌شده را می‌دهد.</summary>
        Task<FetchResult<IReadOnlyList<SymbolInfo>>> FetchSymbolsAsync(CancellationToken cancellationToken);
    }
}
