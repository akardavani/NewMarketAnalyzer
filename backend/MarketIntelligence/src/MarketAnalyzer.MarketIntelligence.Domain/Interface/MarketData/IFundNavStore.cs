namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>ذخیره و خواندن NAV صندوق‌ها.</summary>
    public interface IFundNavStore
    {
        /// <summary>نقاط NAV را Upsert می‌کند (کلید: نماد + روز).</summary>
        Task<UpsertStats> UpsertAsync(IReadOnlyCollection<FundNavPoint> points, CancellationToken cancellationToken);
    }
}
