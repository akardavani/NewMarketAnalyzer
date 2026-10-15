using System;
using System.Buffers;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.DataProvider.Json
{
    internal class TseJsonConveterDateOnly : JsonConverter<DateOnly>
    {
        private const string _dateFormat = "yyyyMMdd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new InvalidOperationException();
            }

            ReadOnlySpan<byte> span = stackalloc byte[0];
            if (reader.HasValueSequence)
            {
                var sequenceLength = reader.ValueSequence.Length;
                Span<byte> stackSpan = stackalloc byte[(int)sequenceLength];
                reader.ValueSequence.CopyTo(stackSpan);
                span = stackSpan;
            }
            else
            {
                span = reader.ValueSpan;
            }
            return DateOnly.ParseExact(Encoding.ASCII.GetString(span), format: _dateFormat, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
