using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace MarketAnalyzer.Persistence.ValueConversions
{
    internal static class JsonHelper
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All, UnicodeRanges.All)
        };

        public static T Deserialize<T>(string json) where T : class
        {
            return string.IsNullOrWhiteSpace(json) ?
                null :
                JsonSerializer.Deserialize<T>(json, _options);
        }

        public static string Serialize<T>(T obj) where T : class
        {
            return obj == null ?
                null :
                JsonSerializer.Serialize(obj, _options);
        }
    }
}

