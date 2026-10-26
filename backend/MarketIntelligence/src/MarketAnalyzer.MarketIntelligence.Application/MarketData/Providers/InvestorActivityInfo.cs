namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>خرید و فروش حقیقی و حقوقی یک نماد در یک روز آن‌طور که Provider گزارش می‌کند.</summary>
    public sealed class InvestorActivityInfo
    {
        /// <summary>کلید استاندارد نماد.</summary>
        public required string SymbolKey { get; init; }

        /// <summary>روز معاملاتی به تقویم محلی بازار.</summary>
        public required DateOnly TradingDate { get; init; }

        /// <summary>حجم خرید حقیقی.</summary>
        public decimal IndividualBuyVolume { get; init; }

        /// <summary>ارزش خرید حقیقی.</summary>
        public decimal IndividualBuyValue { get; init; }

        /// <summary>تعداد خریداران حقیقی.</summary>
        public long IndividualBuyCount { get; init; }

        /// <summary>حجم فروش حقیقی.</summary>
        public decimal IndividualSellVolume { get; init; }

        /// <summary>ارزش فروش حقیقی.</summary>
        public decimal IndividualSellValue { get; init; }

        /// <summary>تعداد فروشندگان حقیقی.</summary>
        public long IndividualSellCount { get; init; }

        /// <summary>حجم خرید حقوقی.</summary>
        public decimal InstitutionalBuyVolume { get; init; }

        /// <summary>ارزش خرید حقوقی.</summary>
        public decimal InstitutionalBuyValue { get; init; }

        /// <summary>تعداد خریداران حقوقی.</summary>
        public long InstitutionalBuyCount { get; init; }

        /// <summary>حجم فروش حقوقی.</summary>
        public decimal InstitutionalSellVolume { get; init; }

        /// <summary>ارزش فروش حقوقی.</summary>
        public decimal InstitutionalSellValue { get; init; }

        /// <summary>تعداد فروشندگان حقوقی.</summary>
        public long InstitutionalSellCount { get; init; }
    }
}
