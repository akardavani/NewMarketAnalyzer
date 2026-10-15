namespace MarketAnalyzer.Domain
{
    public class TseBestLimit : IEntity
    {
        public string Id { get; set; }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Number { get; set; }
        public long InsCode { get; set; }
        /// <summary>
        /// حجم خرید
        /// </summary>
        public long BuyVolume { get; set; }
        /// <summary>
        /// تعداد خرید
        /// </summary>
        public long BuyQuantity { get; set; }
        /// <summary>
        /// تعداد درخواست خرید
        /// </summary>
        public long BuyRequestQuantity { get; set; }
        /// <summary>
        /// حجم فروش
        /// </summary>
        public long SellVolume { get; set; }
        /// <summary>
        /// تعداد فروش
        /// </summary>
        public long SellQuantity { get; set; }
        /// <summary>
        /// تعداد درخواست فروش
        /// </summary>
        public long SellRequestQuantity { get; set; }
        public DateTime Date { get; set; }
    }
}
