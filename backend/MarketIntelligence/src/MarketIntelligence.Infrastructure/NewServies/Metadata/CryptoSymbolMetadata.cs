namespace MarketIntelligence.Infrastructure
{
    public sealed class CryptoSymbolMetadata
    {
        public string? BaseCurrency { get; set; }
        public string? QuoteCurrency { get; set; }
        public decimal? LastTradedPrice { get; set; }

        public decimal? Vol { get; set; }

        public decimal? VolValue { get; set; }        
    }
}



