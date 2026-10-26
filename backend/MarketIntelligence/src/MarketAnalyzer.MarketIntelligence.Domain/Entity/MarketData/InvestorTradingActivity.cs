namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>
    /// خرید و فروش حقیقی و حقوقی یک نماد در یک روز معاملاتی.
    /// حرف I در نام‌های قدیمی TSETMC به «حقیقی» و N به «حقوقی» اشاره دارد.
    /// کلید یکتا: (SymbolId, TradingDate).
    /// </summary>
    public class InvestorTradingActivity
    {
        /// <summary>شناسهٔ ردیف (Identity).</summary>
        public long Id { get; set; }

        /// <summary>شناسهٔ نماد.</summary>
        public int SymbolId { get; set; }

        /// <summary>روز معاملاتی (به تقویم محلی بازار).</summary>
        public DateOnly TradingDate { get; set; }

        /// <summary>حجم خرید حقیقی.</summary>
        public decimal IndividualBuyVolume { get; set; }

        /// <summary>ارزش خرید حقیقی.</summary>
        public decimal IndividualBuyValue { get; set; }

        /// <summary>تعداد خریداران حقیقی.</summary>
        public long IndividualBuyCount { get; set; }

        /// <summary>حجم فروش حقیقی.</summary>
        public decimal IndividualSellVolume { get; set; }

        /// <summary>ارزش فروش حقیقی.</summary>
        public decimal IndividualSellValue { get; set; }

        /// <summary>تعداد فروشندگان حقیقی.</summary>
        public long IndividualSellCount { get; set; }

        /// <summary>حجم خرید حقوقی.</summary>
        public decimal InstitutionalBuyVolume { get; set; }

        /// <summary>ارزش خرید حقوقی.</summary>
        public decimal InstitutionalBuyValue { get; set; }

        /// <summary>تعداد خریداران حقوقی.</summary>
        public long InstitutionalBuyCount { get; set; }

        /// <summary>حجم فروش حقوقی.</summary>
        public decimal InstitutionalSellVolume { get; set; }

        /// <summary>ارزش فروش حقوقی.</summary>
        public decimal InstitutionalSellValue { get; set; }

        /// <summary>تعداد فروشندگان حقوقی.</summary>
        public long InstitutionalSellCount { get; set; }

        /// <summary>زمان دریافت توسط سیستم (UTC).</summary>
        public DateTime ReceivedAtUtc { get; set; }

        /// <summary>نماد مربوط.</summary>
        public MarketSymbol Symbol { get; set; } = null!;
    }
}
