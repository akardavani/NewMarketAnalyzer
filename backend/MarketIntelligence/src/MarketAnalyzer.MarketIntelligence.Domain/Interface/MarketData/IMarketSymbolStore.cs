namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>ذخیره و خواندن کاتالوگ نمادها.</summary>
    public interface IMarketSymbolStore
    {
        /// <summary>نمادهای یک بازار (فقط خواندنی، بدون tracking).</summary>
        Task<IReadOnlyList<MarketSymbol>> GetByMarketAsync(int marketId, bool onlyActive, CancellationToken cancellationToken);

        /// <summary>
        /// نمادهای دریافت‌شده را Upsert می‌کند. اگر <paramref name="deactivateMissing"/> درست باشد، نمادهایی که در ورودی
        /// نیستند غیرفعال می‌شوند؛ فقط وقتی true بدهید که دریافت فهرست کامل و موفق (Success) بوده است.
        /// </summary>
        Task<SymbolSyncResult> SyncAsync(int marketId, IReadOnlyCollection<MarketSymbol> symbols, bool deactivateMissing, CancellationToken cancellationToken);
    }
}
