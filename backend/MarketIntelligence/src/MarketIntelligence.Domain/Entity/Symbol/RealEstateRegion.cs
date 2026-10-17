namespace MarketIntelligence.Domain
{
    public class RealEstateRegion 
    {
        public long Id { get; set; }
        public string Province { get; set; }
        public string Region { get; set; }
        public string Zone { get; set; }
        public string Name { get; set; }
        public long GlobalLocationId { get; set; }
        public string ContinueUrl { get; set; }
    }
}
