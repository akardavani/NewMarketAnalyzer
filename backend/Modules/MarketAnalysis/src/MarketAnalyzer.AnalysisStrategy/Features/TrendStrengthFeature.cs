using MarketAnalyzer.Analysis.Infrastructure;

namespace MarketAnalyzer.AnalysisStrategy
{
    public sealed class TrendStrengthFeature : IFeature
    {
        public string Name => FeaturesName.TrendStrength;

        public decimal Compute(StockAnalysisInput input, FeatureSet features)
        {
            var price = input.Data.DailyCandles.Last().Close;

            var sma50 = input.Data.ExcelData.SMA50;
            var sma200 = input.Data.ExcelData.SMA200;

            decimal score = 0;

            if (price > sma50) score += 0.5m;
            if (sma50 > sma200) score += 0.5m;

            return score;
        }
    }
}
