using System.Text.Json.Serialization;

namespace MarketAnalyzer.Sejam
{
    public class SejamError
    {
        [JsonPropertyName("errorCode")]
        public int Code { get; set; }
        [JsonPropertyName("customMessage")]
        public string Description { get; set; }
    }
}