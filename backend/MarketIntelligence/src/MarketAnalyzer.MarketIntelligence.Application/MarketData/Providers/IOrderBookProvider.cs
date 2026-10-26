using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>قابلیت دریافت صف خرید و فروش نمادها.</summary>
    public interface IOrderBookProvider : IMarketDataProvider
    {
        /// <summary>Snapshot صف نمادها را برمی‌گرداند؛ هر Snapshot باید کامل باشد.</summary>
        Task<FetchResult<IReadOnlyList<OrderBookSnapshot>>> FetchOrderBooksAsync(CancellationToken cancellationToken);
    }
}
