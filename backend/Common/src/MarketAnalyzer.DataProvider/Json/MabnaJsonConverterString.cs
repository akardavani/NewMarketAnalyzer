using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.DataProvider.Json
{
    internal class MabnaJsonConverterString : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            var value = reader.ValueSpan;
            return Encoding.UTF8.GetString(value);
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            // For performance, lift up the writer implementation.
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value.AsSpan());
            }
        }
    }
}
