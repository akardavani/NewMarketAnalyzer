namespace MarketAnalyzer.Domain
{
    public class ImeSymbol
    {
        /// <summary>
        /// عددی هست که تو سرویس بر اساس این سمبول رو میشناسه
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// نماد
        /// </summary>
        public string Symbol { get; set; }
        /// <summary>
        /// اسم نماد
        /// </summary>
        public string PersianName { get; set; }
        /// <summary>
        /// تاریخ فارسی
        /// </summary>
        public string PersianDeliveryDate { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateOnly DeliveryDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ParentSymbol { get; set; }
    }
}
