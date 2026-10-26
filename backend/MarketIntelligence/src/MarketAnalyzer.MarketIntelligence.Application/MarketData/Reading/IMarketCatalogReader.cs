namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Reading
{
    /// <summary>خواندن بازارها و جست‌وجوی نماد برای سمت استفاده (Query، استراتژی، داشبورد).</summary>
    public interface IMarketCatalogReader
    {
        /// <summary>فهرست بازارهای فعال.</summary>
        Task<IReadOnlyList<MarketInfo>> GetMarketsAsync(CancellationToken cancellationToken);

        /// <summary>جست‌وجوی نماد در کاتالوگ.</summary>
        Task<IReadOnlyList<SymbolRef>> SearchSymbolsAsync(SymbolSearchQuery query, CancellationToken cancellationToken);
    }
}
