namespace MarketIntelligence.Application
{
    public class CandleSeriesResult
    {
        public string InstrumentId { get; set; }
        public string Symbol { get; set; }
        public List<CandleResult> Results { get; set; }
    }
}
