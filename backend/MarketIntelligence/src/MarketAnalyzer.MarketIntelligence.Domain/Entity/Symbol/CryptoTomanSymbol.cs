namespace MarketAnalyzer.MarketIntelligence.Domain
{
    public class CryptoTomanSymbol 
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
        /// 
        /// </summary>
        public string ParentSymbol { get; set; }
    }
}
