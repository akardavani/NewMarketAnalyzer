namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>ذخیره و خواندن رویدادهای شرکتی برای مسیر تعدیل Raw و ثبت drift.</summary>
    public interface ICorporateActionStore
    {
        /// <summary>رویدادهای یک نماد را Upsert می‌کند (کلید: نماد + نوع + تاریخ اجرا).</summary>
        Task<UpsertStats> UpsertAsync(int symbolId, IReadOnlyCollection<CorporateAction> actions, CancellationToken cancellationToken);

        /// <summary>رویدادهای یک نماد به ترتیب تاریخ اجرا (فقط خواندنی).</summary>
        Task<IReadOnlyList<CorporateAction>> GetBySymbolAsync(int symbolId, CancellationToken cancellationToken);
    }
}
