namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>نوع رویداد شرکتی که روی قیمت سهم اثر می‌گذارد و باعث تعدیل می‌شود.</summary>
    public enum CorporateActionType
    {
        /// <summary>افزایش سرمایه.</summary>
        CapitalIncrease = 1,

        /// <summary>سود نقدی.</summary>
        CashDividend = 2,

        /// <summary>سایر رویدادها؛ فقط ضریب تعدیل اهمیت دارد.</summary>
        Other = 99
    }
}
