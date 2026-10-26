namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>ذخیره و خواندن خرید و فروش حقیقی و حقوقی روزانه.</summary>
    public interface IInvestorActivityStore
    {
        /// <summary>رکوردها را Upsert می‌کند (کلید: نماد + روز معاملاتی).</summary>
        Task<UpsertStats> UpsertAsync(IReadOnlyCollection<InvestorTradingActivity> activities, CancellationToken cancellationToken);
    }
}
