namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>ذخیره و خواندن کندل‌های بسته‌شدهٔ تاریخی. کندل ناتمام امروز اینجا ذخیره نمی‌شود (جایش CurrentSymbolStates است).</summary>
    public interface IHistoricalCandleStore
    {
        /// <summary>آخرین N کندل یک نماد به ترتیب زمان صعودی (برای پنجرهٔ هم‌پوشانی و تشخیص drift).</summary>
        Task<IReadOnlyList<HistoricalCandle>> GetLastAsync(int symbolId, Timeframe timeframe, int count, CancellationToken cancellationToken);

        /// <summary>کندل‌های یک نماد از زمان داده‌شده به بعد، به ترتیب صعودی (برای ساخت هفتگی/ماهانه).</summary>
        Task<IReadOnlyList<HistoricalCandle>> GetFromAsync(int symbolId, Timeframe timeframe, DateTime fromUtc, CancellationToken cancellationToken);

        /// <summary>
        /// کندل‌ها را Upsert می‌کند (کلید: نماد + تایم‌فریم + OpenTimeUtc). ورودی تکراری یکتا می‌شود (آخری برنده).
        /// در تداخل همزمان با نویسندهٔ دیگر یک‌بار دوباره تلاش می‌کند.
        /// </summary>
        Task<UpsertStats> UpsertAsync(int symbolId, Timeframe timeframe, IReadOnlyCollection<HistoricalCandle> candles, CancellationToken cancellationToken);

        /// <summary>
        /// کل سری یک نماد و تایم‌فریم را در یک تراکنش اتمی جایگزین می‌کند (حذف + درج). اگر درج شکست بخورد
        /// سری قبلی دست‌نخورده می‌ماند. فقط بعد از دریافت کاملاً موفق صدا زده شود.
        /// </summary>
        Task ReplaceAllAsync(int symbolId, Timeframe timeframe, IReadOnlyCollection<HistoricalCandle> candles, CancellationToken cancellationToken);

        /// <summary>
        /// چند تایم‌فریم یک نماد (مثلاً روزانه + هفتگی + ماهانه) را در <b>یک</b> تراکنش جایگزین می‌کند تا سری‌های مشتق
        /// هرگز با سری پایه ناهمخوان نشوند. هر مجموعهٔ خالی خطا است (جایگزینی فقط با داده).
        /// </summary>
        Task ReplaceSeriesAsync(int symbolId, IReadOnlyDictionary<Timeframe, IReadOnlyCollection<HistoricalCandle>> candlesByTimeframe, CancellationToken cancellationToken);

        /// <summary>
        /// کندل‌های مشخص‌شده با OpenTimeUtc را حذف می‌کند (برای کندلی که منبع دیگر نمی‌دهد). تعداد حذف‌شده را برمی‌گرداند.
        /// </summary>
        Task<int> DeleteByOpenTimesAsync(int symbolId, Timeframe timeframe, IReadOnlyCollection<DateTime> openTimesUtc, CancellationToken cancellationToken);

        /// <summary>کندل‌های یک نماد و تایم‌فریم را از زمان داده‌شده به بعد حذف می‌کند و تعداد حذف‌شده را برمی‌گرداند.</summary>
        Task<int> DeleteFromAsync(int symbolId, Timeframe timeframe, DateTime fromUtc, CancellationToken cancellationToken);
    }
}
