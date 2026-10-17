namespace MarketAnalyzer.AnalysisStrategy
{
    public sealed class ValueFactor : IFactor
    {
        public string Name => "Value";

        public decimal Score(StockAnalysisInput input, FeatureSet features)
        {
            var pe = input.Data.ExcelData.PE;
            var eps = input.Data.ExcelData.EPS;

            if (pe <= 0)
                return 0;

            decimal score = 0;

            if (pe < 5) score = 1m;
            else if (pe < 8) score = 0.8m;
            else if (pe < 12) score = 0.6m;
            else if (pe < 16) score = 0.4m;
            else score = 0.2m;

            return score;
        }
    }
}
