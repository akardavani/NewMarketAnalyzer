namespace MarketAnalyzer.Domain
{
    public class TseStaticThreshold : IEntity
    {
        public string Id { get; set; }

        public long InsCode { get; set; }
        public decimal MaxRange { get; set; }
        public decimal MinRange { get; set; }
        public DateTime Date { get; set; }
    }
}
