using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>قابلیت دریافت رویدادهای شرکتی (افزایش سرمایه و سود) برای مسیر تعدیل Raw.</summary>
    public interface ICorporateActionProvider : IMarketDataProvider
    {
        /// <summary>رویدادهای شرکتی را برمی‌گرداند.</summary>
        Task<FetchResult<IReadOnlyList<CorporateActionInfo>>> FetchCorporateActionsAsync(CancellationToken cancellationToken);
    }
}
