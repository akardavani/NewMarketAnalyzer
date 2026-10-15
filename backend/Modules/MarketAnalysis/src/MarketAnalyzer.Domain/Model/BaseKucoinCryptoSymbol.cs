using Newtonsoft.Json;

namespace MarketAnalyzer.Domain
{
    public class BaseKucoinCryptoSymbol
    {
        public int Id { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("symbolCode")]
        public string SymbolCode { get; set; }

        [JsonProperty("vol")]
        public string Vol { get; set; }

        [JsonProperty("volValue")]
        public string VolValue { get; set; }

        [JsonProperty("close")]
        public string Close { get; set; }

        [JsonProperty("baseCurrency")]
        public string BaseCurrency { get; set; }

        [JsonProperty("quoteCurrency")]
        public string QuoteCurrency { get; set; }

        [JsonProperty("trading")]
        public bool Trading { get; set; }

        [JsonProperty("market")]
        public string Market { get; set; }

        [JsonProperty("lastTradedPrice")]
        public string LastTradedPrice { get; set; }
    }
}
