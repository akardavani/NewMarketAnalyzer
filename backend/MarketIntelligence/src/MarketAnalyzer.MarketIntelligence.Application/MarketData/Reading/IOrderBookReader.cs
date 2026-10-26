namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Reading
{
    /// <summary>خواندن Snapshot صف خرید و فروش یک نماد.</summary>
    public interface IOrderBookReader
    {
        /// <summary>صف نماد را می‌خواند؛ اگر صفی ذخیره نشده باشد null برمی‌گرداند.</summary>
        Task<OrderBookDto?> GetOrderBookAsync(int symbolId, CancellationToken cancellationToken);
    }
}
