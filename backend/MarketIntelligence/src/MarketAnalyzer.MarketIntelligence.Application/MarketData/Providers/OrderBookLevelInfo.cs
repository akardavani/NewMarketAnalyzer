using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>یک سطح از صف خرید یا فروش آن‌طور که Provider گزارش می‌کند.</summary>
    public sealed class OrderBookLevelInfo
    {
        /// <summary>سمت خرید یا فروش.</summary>
        public required OrderBookSide Side { get; init; }

        /// <summary>شمارهٔ سطح از ۱ (بهترین قیمت).</summary>
        public required int LevelNumber { get; init; }

        /// <summary>قیمت سطح.</summary>
        public required decimal Price { get; init; }

        /// <summary>حجم سفارش‌های سطح.</summary>
        public required decimal Volume { get; init; }

        /// <summary>تعداد سفارش‌های سطح.</summary>
        public required int OrderCount { get; init; }
    }
}
