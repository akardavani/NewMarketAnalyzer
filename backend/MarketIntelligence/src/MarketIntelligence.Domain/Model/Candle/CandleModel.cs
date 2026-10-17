namespace MarketIntelligence.Domain
{
    public class CandleModel
    {
        // required base properties        
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Volume { get; set; }

        // custom properties
        public long UnixTimestamp { get; set; }
        public long Timestamp { get; set; }
        public decimal NetValue { get; set; }

    }
}
