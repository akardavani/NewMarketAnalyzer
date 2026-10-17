namespace MarketIntelligence.Application
{
    public class YtmCandle
    {
        public string InstrumentId { get; set; }
        public decimal Close { get; set; }
        public int MaturityDate { get; set; }
        public DateTime Date { get; set; }
    }
}

