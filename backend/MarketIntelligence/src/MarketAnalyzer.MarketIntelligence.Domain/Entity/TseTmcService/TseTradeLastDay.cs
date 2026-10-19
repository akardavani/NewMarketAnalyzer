namespace MarketAnalyzer.MarketIntelligence.Domain
{
    public class TseTradeLastDay : IEntity
    {
        public string Id { get; set; }

        public long InsCode { get; set; }
        public DateTime Date { get; set; }
        public int Time { get; set; }
        public decimal PriceFirst { get; set; }
        public decimal Closing { get; set; }
        public decimal LastPrice { get; set; }
        /// <summary>
        /// تعداد معاملات
        /// </summary>
        public long Count { get; set; }
        /// <summary>
        /// حجم معاملات
        /// </summary>
        public long Volume { get; set; }
        /// <summary>
        /// ارزش معاملات
        /// </summary>
        public decimal TransactionValue { get; set; }

        public decimal PriceChange { get; set; }
        /// <summary>
        /// کمترین قیمت
        /// </summary>
        public decimal PriceMin { get; set; }
        /// <summary>
        /// بیشترین قیمت
        /// </summary>        
        public decimal PriceMax { get; set; }
        /// <summary>
        /// قیمت دیروز
        /// </summary>
        public decimal PriceYesterday { get; set; }
        /// <summary>
        /// آیا آخرین معامله است؟
        /// </summary>
        public int Last { get; set; }
    }
}
