namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <summary>انواع کارهای دوره‌ای دریافت داده برای یک بازار.</summary>
    public enum MarketJobKind
    {
        /// <summary>همگام‌سازی فهرست نمادهای بازار.</summary>
        SyncCatalog = 1,

        /// <summary>دریافت و ذخیرهٔ کندل‌های تاریخی (روزانه و مشتقات).</summary>
        IngestCandles = 2,

        /// <summary>به‌روزرسانی وضعیت لحظه‌ای نمادها.</summary>
        RefreshCurrentState = 3,

        /// <summary>به‌روزرسانی صف سفارش‌ها.</summary>
        RefreshOrderBook = 4,

        /// <summary>به‌روزرسانی فعالیت حقیقی/حقوقی.</summary>
        RefreshInvestorActivity = 5,

        /// <summary>دریافت ارزش خالص دارایی (NAV) صندوق‌ها.</summary>
        RefreshFundNav = 6
    }
}
