namespace MarketIntelligence.Infrastructure
{
    public class MarketQuery
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string>? Symbols { get; set; }
        public int? Timeframe { get; set; }
    }
}
