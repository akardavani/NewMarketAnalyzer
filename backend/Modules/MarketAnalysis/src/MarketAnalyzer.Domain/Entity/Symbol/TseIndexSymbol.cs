namespace MarketAnalyzer.Domain
{
    public class TseIndexSymbol
    {
        public long Id { get; set; }

        public string InsCode { get; set; }
        public string SectorCode { get; set; }
        public string SectorName { get; set; }
        public string InstrumentId { get; set; }
        public long Flow { get; set; }     
    }
}