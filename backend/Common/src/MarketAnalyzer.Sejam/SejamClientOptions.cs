using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.Sejam
{
    public class SejamClientOptions
    {
        private static readonly JsonSerializerOptions _defaultSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        };

        public string BaseUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Version { get; set; }
        public JsonSerializerOptions SerializerOptions { get; set; } = _defaultSerializerOptions;
    }
}