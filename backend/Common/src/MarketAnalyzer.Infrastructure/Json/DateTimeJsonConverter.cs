using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.Infrastructure.Json
{
    internal sealed class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString(), CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var jsDate = $"/D({ToJavaScriptMilliseconds(value.ToUniversalTime())})/";
            writer.WriteStringValue(jsDate);
        }

        private static long ToJavaScriptMilliseconds(DateTime value)
        {
            return (long)(value - DateTime.UnixEpoch).TotalMilliseconds;
        }
    }
}
