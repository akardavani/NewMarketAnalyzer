using System.Text.Json;

namespace MarketAnalyzer.Analysis.Infrastructure
{
    internal static class UnifiedSymbolJsonMapper
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public static string? ToJson<T>(T metadata)
        {
            if (metadata == null)
                return null;

            return JsonSerializer.Serialize(metadata, Options);
        }
        public static T? FromJson<T>(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }

    }
}



