using System.Text.Json;

namespace MarketAnalyzer.DataProvider.Json
{
    internal class ImeJsonOptions
    {
        public static readonly ImeJsonOptions Default = new();

        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ImeJsonOptions()
        {
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public JsonSerializerOptions SerializerOptions => _jsonSerializerOptions;
    }
}
