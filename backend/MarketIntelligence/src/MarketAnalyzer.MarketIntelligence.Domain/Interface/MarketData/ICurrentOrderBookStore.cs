namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>ذخیره و خواندن Snapshot صف خرید و فروش؛ فقط آخرین Snapshot هر نماد نگه‌داری می‌شود.</summary>
    public interface ICurrentOrderBookStore
    {
        /// <summary>
        /// صف هر نماد را به‌طور کامل جایگزین می‌کند. همهٔ نمادها در یک تراکنش‌اند و برای هر نماد سطح‌های قدیمی
        /// و جدید هرگز با هم مخلوط نمی‌شوند. فقط Snapshot کامل و معتبر پاس داده شود.
        /// </summary>
        Task ReplaceAsync(IReadOnlyDictionary<int, IReadOnlyList<CurrentOrderBookLevel>> snapshotsBySymbolId, CancellationToken cancellationToken);

        /// <summary>سطح‌های صف یک نماد (فقط خواندنی) به ترتیب سمت و شمارهٔ سطح.</summary>
        Task<IReadOnlyList<CurrentOrderBookLevel>> GetAsync(int symbolId, CancellationToken cancellationToken);
    }
}
