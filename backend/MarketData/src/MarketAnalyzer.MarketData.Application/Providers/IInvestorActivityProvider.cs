using MarketAnalyzer.MarketData.Domain;

namespace MarketAnalyzer.MarketData.Application.Providers
{
    /// <summary>قابلیت دریافت خرید و فروش حقیقی و حقوقی روزانه.</summary>
    public interface IInvestorActivityProvider : IMarketDataProvider
    {
        /// <summary>فعالیت حقیقی و حقوقی نمادها را برمی‌گرداند.</summary>
        Task<FetchResult<IReadOnlyList<InvestorActivityInfo>>> FetchInvestorActivitiesAsync(CancellationToken cancellationToken);
    }
}
