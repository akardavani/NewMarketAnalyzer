namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>
    /// قابلیت‌هایی که یک بازار می‌تواند از Providerها بگیرد. منبع فعال برای هر (بازار، قابلیت) جداگانه انتخاب می‌شود،
    /// چون در یک بازار منبع فهرست نماد و منبع کندل ممکن است فرق کنند (مثلاً بورس ایران: فهرست از TSETMC، کندل از نهایت‌نگر).
    /// </summary>
    public enum ProviderCapability
    {
        /// <summary>فهرست نمادها (<see cref="ISymbolCatalogProvider"/>).</summary>
        SymbolCatalog = 1,

        /// <summary>کندل تاریخی (<see cref="IHistoricalCandleProvider"/>).</summary>
        HistoricalCandles = 2,

        /// <summary>آخرین قیمت و وضعیت (<see cref="ICurrentStateProvider"/>).</summary>
        CurrentState = 3,

        /// <summary>صف خرید و فروش (<see cref="IOrderBookProvider"/>).</summary>
        OrderBook = 4,

        /// <summary>رویداد شرکتی (<see cref="ICorporateActionProvider"/>).</summary>
        CorporateActions = 5,

        /// <summary>خرید و فروش حقیقی/حقوقی (<see cref="IInvestorActivityProvider"/>).</summary>
        InvestorActivity = 6,

        /// <summary>NAV صندوق (<see cref="IFundNavProvider"/>).</summary>
        FundNav = 7
    }
}
