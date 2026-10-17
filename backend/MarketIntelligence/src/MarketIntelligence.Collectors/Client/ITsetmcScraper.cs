namespace MarketIntelligence.Collectors
{
    public interface ITsetmcScraper
    {
        Task<InstrumentInfo> GetInstrumentAsync(string url);
    }


}
