using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <summary>تشخیص قابلیت‌های یک Provider از روی رابط‌هایی که پیاده می‌کند.</summary>
    public static class ProviderCapabilities
    {
        /// <summary>قابلیت‌های Provider.</summary>
        public static IReadOnlyList<ProviderCapability> Of(IMarketDataProvider provider)
        {
            var list = new List<ProviderCapability>(2);
            if (provider is ISymbolCatalogProvider) list.Add(ProviderCapability.SymbolCatalog);
            if (provider is IHistoricalCandleProvider) list.Add(ProviderCapability.HistoricalCandles);
            if (provider is ICurrentStateProvider) list.Add(ProviderCapability.CurrentState);
            if (provider is IOrderBookProvider) list.Add(ProviderCapability.OrderBook);
            if (provider is ICorporateActionProvider) list.Add(ProviderCapability.CorporateActions);
            if (provider is IInvestorActivityProvider) list.Add(ProviderCapability.InvestorActivity);
            if (provider is IFundNavProvider) list.Add(ProviderCapability.FundNav);
            return list;
        }

        /// <summary>قابلیتی که یک کار دوره‌ای به آن نیاز دارد.</summary>
        public static ProviderCapability For(MarketJobKind job) => job switch
        {
            MarketJobKind.SyncCatalog => ProviderCapability.SymbolCatalog,
            MarketJobKind.IngestCandles => ProviderCapability.HistoricalCandles,
            MarketJobKind.RefreshCurrentState => ProviderCapability.CurrentState,
            MarketJobKind.RefreshOrderBook => ProviderCapability.OrderBook,
            MarketJobKind.RefreshInvestorActivity => ProviderCapability.InvestorActivity,
            _ => throw new ArgumentOutOfRangeException(nameof(job), job, null)
        };
    }
}
