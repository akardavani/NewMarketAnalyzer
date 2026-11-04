namespace MarketAnalyzer.MarketData.Application.Reading
{
    /// <summary>تنها نقطهٔ مجاز خواندن کندل: همیشه سری تعدیل‌شده را برمی‌گرداند. هیچ مصرف‌کننده‌ای مستقیم به HistoricalCandles Query نمی‌زند.</summary>
    public interface IAdjustedCandleReader
    {
        /// <summary>سری کندل تعدیل‌شدهٔ یک نماد را می‌خواند؛ اگر نماد یا داده‌ای نباشد null برمی‌گرداند.</summary>
        Task<AdjustedCandleSeries?> GetSeriesAsync(CandleQuery query, CancellationToken cancellationToken);

        /// <summary>سری نمادهای خواسته‌شده را با خواندن گروهی برمی‌گرداند؛ خروجی برای هر نماد جداگانه مرتب است.</summary>
        Task<IReadOnlyList<AdjustedCandleSeries>> GetSeriesAsync(MultiSymbolCandleQuery query, CancellationToken cancellationToken);
    }
}
