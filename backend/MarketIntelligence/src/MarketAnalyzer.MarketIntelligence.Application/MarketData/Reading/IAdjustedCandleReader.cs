namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Reading
{
    /// <summary>تنها نقطهٔ مجاز خواندن کندل: همیشه سری تعدیل‌شده را برمی‌گرداند. هیچ مصرف‌کننده‌ای مستقیم به HistoricalCandles Query نمی‌زند.</summary>
    public interface IAdjustedCandleReader
    {
        /// <summary>سری کندل تعدیل‌شدهٔ یک نماد را می‌خواند؛ اگر نماد یا داده‌ای نباشد null برمی‌گرداند.</summary>
        Task<AdjustedCandleSeries?> GetSeriesAsync(CandleQuery query, CancellationToken cancellationToken);
    }
}
