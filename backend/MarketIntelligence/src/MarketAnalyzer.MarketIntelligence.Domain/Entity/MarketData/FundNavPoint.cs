namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>
    /// یک نقطه از سری NAV صندوق در یک روز؛ جایگزین InvestmentFundChart و TseTmcETFNav.
    /// کلید یکتا: (SymbolId, Date).
    /// </summary>
    public class FundNavPoint
    {
        /// <summary>شناسهٔ ردیف (Identity).</summary>
        public long Id { get; set; }

        /// <summary>شناسهٔ نمادِ صندوق.</summary>
        public int SymbolId { get; set; }

        /// <summary>روز مربوط به NAV.</summary>
        public DateOnly Date { get; set; }

        /// <summary>NAV صدور.</summary>
        public decimal? IssuanceNav { get; set; }

        /// <summary>NAV ابطال.</summary>
        public decimal? CancellationNav { get; set; }

        /// <summary>NAV آماری.</summary>
        public decimal? StatisticalNav { get; set; }

        /// <summary>زمان دریافت توسط سیستم (UTC).</summary>
        public DateTime ReceivedAtUtc { get; set; }

        /// <summary>نماد صندوق.</summary>
        public MarketSymbol Symbol { get; set; } = null!;
    }
}
