using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>قابلیت دریافت سری NAV صندوق‌ها.</summary>
    public interface IFundNavProvider : IMarketDataProvider
    {
        /// <summary>نقاط NAV صندوق‌ها را برمی‌گرداند.</summary>
        Task<FetchResult<IReadOnlyList<FundNavInfo>>> FetchFundNavsAsync(CancellationToken cancellationToken);
    }
}
