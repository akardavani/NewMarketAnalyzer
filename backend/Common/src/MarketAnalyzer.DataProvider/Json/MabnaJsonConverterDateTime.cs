using System;
using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.DataProvider.Json
{
    internal class MabnaJsonConverterDateTime : JsonConverter<DateTime>
    {
        private const string _dateFormat = "yyyyMMdd";
        private const string _dateTimeFormat = "yyyyMMddHHmmss";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new InvalidOperationException();
            }

            string format;
            ReadOnlySpan<byte> span = stackalloc byte[0];

            if (reader.HasValueSequence)
            {
                var sequenceLength = reader.ValueSequence.Length;
                if (!TryGetFormat(sequenceLength, out format))
                {
                    throw new FormatException();
                }

                Span<byte> stackSpan = stackalloc byte[(int)sequenceLength];
                reader.ValueSequence.CopyTo(stackSpan);
                span = stackSpan;
            }
            else
            {
                if (!TryGetFormat(reader.ValueSpan.Length, out format))
                {
                    throw new FormatException();
                }

                span = reader.ValueSpan;
            }

            return DateTime.ParseExact(Encoding.ASCII.GetString(span), format: format, CaltureHelper.FaCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private static bool TryGetFormat(long length, out string format)
        {
            if (length == _dateFormat.Length)
            {
                format = _dateFormat;
                return true;
            }
            else if (length == _dateTimeFormat.Length)
            {
                format = _dateTimeFormat;
                return true;
            }

            format = null;
            return false;
        }
    }
}
