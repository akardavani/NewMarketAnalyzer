namespace MarketAnalyzer.Domain
{
    public class CandleSeries
    {
        public string Symbol { get; set; }
        public List<CandleModel> Candels { get; set; }
        public Timeframe Timeframe { get; set; }
        public int marketType { get; set; }
    }
}
