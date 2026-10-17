namespace MarketAnalyzer.Domain
{
    public class MacroSymbol 
    {
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
        /// نوع
        /// </summary>
        public int GoldType { get; set; }
        /// <summary>
        /// بازار
        /// </summary>
        public int MarketType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ParentSymbol { get; set; }
    }
}
