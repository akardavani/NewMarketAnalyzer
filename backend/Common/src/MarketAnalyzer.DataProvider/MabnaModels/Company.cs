namespace MarketAnalyzer.DataProvider.MabnaModels
{
    public class Company : EntityBase
    {
        /// <summary>
        /// نام شرکت
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// نام انگلیسی شرکت
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// نام کوتاه شرکت
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// نام کوتاه انگلیسی شرکت
        /// </summary>
        public string EnglishShortName { get; set; }

        /// <summary>
        /// نماد معاملاتی
        /// </summary>
        public string TradeSymbol { get; set; }

        /// <summary>
        /// نماد معاملاتی انگلیسی
        /// </summary>
        public string EnglishTradeSymbol { get; set; }

        /// <summary>
        /// توضیحات
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// سال مالی
        /// </summary>
        public string Fiscalyear { get; set; }

        /// <summary>
        /// بازار معاملاتی
        /// </summary>
        public Exchange Exchange { get; set; }
    }
}
