namespace MarketAnalyzer.AnalysisStrategy
{
    public class IranEquityStrategyResult
    {
        public string Symbol { get; set; }
        public decimal FinalScore { get; set; }
        public decimal Percentile { get; set; }
        public string Tier { get; set; }

        // فیلدهای محاسباتی داخلی (Internal)
        internal decimal RawMomentum { get; set; }
        internal decimal RawLiquidity { get; set; }
        internal decimal RawQuality { get; set; }

        // فیلدهای سیگنالینگ
        public decimal? DistanceToSupport { get; set; }
        public decimal? DistanceToResistance { get; set; }
        public decimal LastPrice { get; set; }
        
        public decimal ATR { get; set; }

        public decimal FairValue { get; set; }

        public decimal Entry { get; set; }

        public decimal StopLoss { get; set; }

        public decimal[] TakeProfits { get; set; }

        public Dictionary<string, decimal> FactorScores { get; set; }

        public Dictionary<string, decimal> FeatureValues { get; set; }

        public bool IsCandidate { get; set; }
    }

}
