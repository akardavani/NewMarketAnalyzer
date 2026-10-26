namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>ذخیره و خواندن تعریف بازارها. ردیف بازار از روی ProviderDescriptor ساخته می‌شود و seed دستی ندارد.</summary>
    public interface IMarketDefinitionStore
    {
        /// <summary>
        /// بازار را اگر نبود می‌سازد و اگر بود نام، واحد پول، تقویم و منطقهٔ زمانی را با مقدار جدید هم‌تراز می‌کند.
        /// وضعیت IsActive را که اپراتور تعیین می‌کند هرگز بازنویسی نمی‌کند.
        /// </summary>
        Task<MarketDefinition> EnsureAsync(MarketDefinition desired, CancellationToken cancellationToken);

        /// <summary>بازار را با کلید می‌خواند؛ اگر نباشد null.</summary>
        Task<MarketDefinition?> GetByKeyAsync(string key, CancellationToken cancellationToken);

        /// <summary>همهٔ بازارها (فقط خواندنی، بدون tracking).</summary>
        Task<IReadOnlyList<MarketDefinition>> GetAllAsync(CancellationToken cancellationToken);
    }
}
