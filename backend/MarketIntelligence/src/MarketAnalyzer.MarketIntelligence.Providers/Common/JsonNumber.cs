using System.Globalization;
using System.Text.Json;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>خواندن عدد از JSON که منبع‌ها گاهی عددی و گاهی رشته‌ای می‌فرستند.</summary>
    internal static class JsonNumber
    {
        public static decimal ReadDecimal(JsonElement e) => e.ValueKind switch
        {
            JsonValueKind.Number => e.GetDecimal(),
            JsonValueKind.String => decimal.Parse(e.GetString()!, NumberStyles.Float, CultureInfo.InvariantCulture),
            _ => throw new FormatException($"Unexpected JSON value kind {e.ValueKind} for a number.")
        };

        public static long ReadLong(JsonElement e) => e.ValueKind switch
        {
            JsonValueKind.Number => e.TryGetInt64(out var l) ? l : (long)e.GetDouble(),
            JsonValueKind.String => (long)double.Parse(e.GetString()!, CultureInfo.InvariantCulture),
            _ => throw new FormatException($"Unexpected JSON value kind {e.ValueKind} for a timestamp.")
        };
    }
}
