using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.Infrastructure.Json
{
    internal sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var date = DateTime.Parse(reader.GetString(), CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind);
            return date.ToDateOnly();
        }

        public override DateOnly ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateOnly.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            var utc = value.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
            var jsDate = $"/D({ToJavaScriptMilliseconds(utc)})/";
            writer.WriteStringValue(jsDate);
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            var isoDate = value.ToString("O");
            writer.WritePropertyName(isoDate);
        }

        private static long ToJavaScriptMilliseconds(DateTime value)
        {
            return (long)(value - DateTime.UnixEpoch).TotalMilliseconds;
        }
    }
}
