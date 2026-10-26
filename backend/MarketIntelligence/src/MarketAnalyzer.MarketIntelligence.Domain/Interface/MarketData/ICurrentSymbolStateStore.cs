namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>ذخیره و خواندن آخرین وضعیت نمادها؛ فقط آخرین مقدار هر نماد نگه‌داری می‌شود.</summary>
    public interface ICurrentSymbolStateStore
    {
        /// <summary>وضعیت‌ها را Upsert می‌کند (کلید: SymbolId). فقط ستون‌هایی که واقعاً تغییر کرده‌اند UPDATE می‌شوند.</summary>
        Task<UpsertStats> UpsertAsync(IReadOnlyCollection<CurrentSymbolState> states, CancellationToken cancellationToken);

        /// <summary>وضعیت نمادهای داده‌شده (فقط خواندنی)؛ نمادهای بدون وضعیت در نتیجه نمی‌آیند.</summary>
        Task<IReadOnlyList<CurrentSymbolState>> GetAsync(IReadOnlyCollection<int> symbolIds, CancellationToken cancellationToken);
    }
}
