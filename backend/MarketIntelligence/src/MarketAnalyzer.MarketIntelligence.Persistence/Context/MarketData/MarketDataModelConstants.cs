namespace MarketAnalyzer.MarketIntelligence.Persistence
{
    /// <summary>
    /// ثابت‌های مشترک نگاشت EF جدول‌های داده بازار (schema و نوع ستون‌ها)
    /// تا در همهٔ Configurationها یکسان باشند و فقط در یک جا تغییر کنند.
    /// </summary>
    public static class MarketDataModelConstants
    {
        /// <summary>نام schema همهٔ جدول‌های MarketIntelligence.</summary>
        public const string Schema = "marketintelligence";

        /// <summary>نوع ستون قیمت؛ ارزهای دیجیتال قیمت‌های بسیار کوچک دارند، پس ۱۰ رقم اعشار.</summary>
        public const string PriceColumnType = "decimal(28,10)";

        /// <summary>نوع ستون حجم و ارزش معاملات.</summary>
        public const string QuantityColumnType = "decimal(28,8)";

        /// <summary>نوع ستون ضریب تعدیل؛ دقت بالا لازم است چون ضرب‌های متوالی خطا جمع می‌کنند.</summary>
        public const string FactorColumnType = "decimal(28,12)";

        /// <summary>نوع ستون زمان با دقت ثانیه.</summary>
        public const string SecondPrecisionDateTime = "datetime2(0)";
    }
}
