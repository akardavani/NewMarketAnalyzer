namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>یک نقطهٔ NAV صندوق آن‌طور که Provider گزارش می‌کند.</summary>
    public sealed class FundNavInfo
    {
        /// <summary>کلید استاندارد نمادِ صندوق.</summary>
        public required string SymbolKey { get; init; }

        /// <summary>روز مربوط به NAV.</summary>
        public required DateOnly Date { get; init; }

        /// <summary>NAV صدور.</summary>
        public decimal? IssuanceNav { get; init; }

        /// <summary>NAV ابطال.</summary>
        public decimal? CancellationNav { get; init; }

        /// <summary>NAV آماری.</summary>
        public decimal? StatisticalNav { get; init; }
    }
}
