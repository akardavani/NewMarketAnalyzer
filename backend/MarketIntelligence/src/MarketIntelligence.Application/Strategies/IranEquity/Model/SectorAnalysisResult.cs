namespace MarketIntelligence.Application
{
    public class SectorAnalysisResult
    {
        public string SectorName { get; set; }
        /// <summary>
        /// میانگین امتیاز نمادهای صنعت
        /// </summary>
        public decimal AverageScore { get; set; }
        /// <summary>
        ///  قدرت روند در صنعت
        /// </summary>
        public decimal MomentumScore { get; set; }
        /// <summary>
        /// مجموع ورود پول حقیقی به صنعت
        /// </summary>
        public decimal TotalMoneyFlow { get; set; }
        /// <summary>
        /// تعداد نمادهای دسته A و A+ در این صنعت
        /// </summary>
        public int TopTierCount { get; set; }
        /// <summary>
        /// Hot / Neutral / Cold
        /// </summary>
        public string Status { get; set; }
        public int SymbolCount { get; set; }
    }
}
