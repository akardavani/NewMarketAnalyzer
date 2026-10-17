using System;
using System.Buffers;
using System.Buffers.Text;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.DataProvider.Json
{
    internal class MabnaJsonConverterStringConverter : JsonConverter<string>
    {
        // Reject any invalid UTF-8 data rather than silently replacing.
        private static readonly UTF8Encoding _utf8Encoding = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.String && reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new Exception($"Cannot get the value of a token type '{reader.TokenType}' as a string.");
            }

            ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
            var result = reader.ValueIsEscaped ? 
                GetUnescapedString(span): 
                TranscodeHelper(span);
            
            return result.Replace("ي", "ی").Replace("ك", "ک").Trim();
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

        private static string GetUnescapedString(ReadOnlySpan<byte> utf8Source)
        {
            // The escaped name is always >= than the unescaped, so it is safe to use escaped name for the buffer length.
            int length = utf8Source.Length;
            byte[] pooledName = null;

            Span<byte> utf8Unescaped = length <= JsonConstants.StackallocByteThreshold ?
                stackalloc byte[JsonConstants.StackallocByteThreshold] :
                (pooledName = ArrayPool<byte>.Shared.Rent(length));

            Unescape(utf8Source, utf8Unescaped, out int written);

            utf8Unescaped = utf8Unescaped.Slice(0, written);

            string utf8String = TranscodeHelper(utf8Unescaped);

            if (pooledName != null)
            {
                utf8Unescaped.Clear();
                ArrayPool<byte>.Shared.Return(pooledName);
            }

            return utf8String;
        }

        private static void Unescape(ReadOnlySpan<byte> source, Span<byte> destination, out int written)
        {
            int idx = source.IndexOf(JsonConstants.BackSlash);
            TryUnescape(source, destination, idx, out written);
        }

        public static string TranscodeHelper(ReadOnlySpan<byte> utf8Unescaped)
        {
            try
            {
                return _utf8Encoding.GetString(utf8Unescaped);
            }
            catch (DecoderFallbackException ex)
            {
                throw new InvalidOperationException("Cannot transcode invalid UTF-8 JSON text to UTF-16 string.", ex);
            }
        }

        private static bool TryUnescape(ReadOnlySpan<byte> source, Span<byte> destination, int idx, out int written)
        {
            if (!source.Slice(0, idx).TryCopyTo(destination))
            {
                written = 0;
                goto DestinationTooShort;
            }

            written = idx;

            while (true)
            {
                if (written == destination.Length)
                {
                    goto DestinationTooShort;
                }

                switch (source[++idx])
                {
                    case JsonConstants.Quote:
                        destination[written++] = JsonConstants.Quote;
                        break;
                    case (byte)'n':
                        destination[written++] = JsonConstants.LineFeed;
                        break;
                    case (byte)'r':
                        destination[written++] = JsonConstants.CarriageReturn;
                        break;
                    case JsonConstants.BackSlash:
                        destination[written++] = JsonConstants.BackSlash;
                        break;
                    case JsonConstants.Slash:
                        destination[written++] = JsonConstants.Slash;
                        break;
                    case (byte)'t':
                        destination[written++] = JsonConstants.Tab;
                        break;
                    case (byte)'b':
                        destination[written++] = JsonConstants.BackSpace;
                        break;
                    case (byte)'f':
                        destination[written++] = JsonConstants.FormFeed;
                        break;
                    default:
                        bool result = Utf8Parser.TryParse(source.Slice(idx + 1, 4), out int scalar, out int bytesConsumed, 'x');
                        idx += 4;

                        if (IsInRangeInclusive((uint)scalar, JsonConstants.HighSurrogateStartValue, JsonConstants.LowSurrogateEndValue))
                        {
                            // The first hex value cannot be a low surrogate.
                            if (scalar >= JsonConstants.LowSurrogateStartValue)
                            {
                                throw new InvalidOperationException();
                            }

                            // We must have a low surrogate following a high surrogate.
                            if (source.Length < idx + 7 || source[idx + 1] != '\\' || source[idx + 2] != 'u')
                            {
                                throw new InvalidOperationException();
                            }

                            // The source is known to be valid JSON, and hence if we see a \u, it is guaranteed to have 4 hex digits following it
                            // Otherwise, the Utf8JsonReader would have already thrown an exception.
                            result = Utf8Parser.TryParse(source.Slice(idx + 3, 4), out int lowSurrogate, out bytesConsumed, 'x');
                            idx += 6;

                            // If the first hex value is a high surrogate, the next one must be a low surrogate.
                            if (!IsInRangeInclusive((uint)lowSurrogate, JsonConstants.LowSurrogateStartValue, JsonConstants.LowSurrogateEndValue))
                            {
                                throw new InvalidOperationException();
                            }

                            // To find the unicode scalar:
                            // (0x400 * (High surrogate - 0xD800)) + Low surrogate - 0xDC00 + 0x10000
                            scalar = (JsonConstants.BitShiftBy10 * (scalar - JsonConstants.HighSurrogateStartValue))
                                + (lowSurrogate - JsonConstants.LowSurrogateStartValue)
                                + JsonConstants.UnicodePlane01StartValue;
                        }
                        var rune = new Rune(scalar);
                        bool success = rune.TryEncodeToUtf8(destination.Slice(written), out int bytesWritten);
                        if (!success)
                        {
                            goto DestinationTooShort;
                        }

                        written += bytesWritten;
                        break;
                }

                if (++idx == source.Length)
                {
                    goto Success;
                }

                if (source[idx] != JsonConstants.BackSlash)
                {
                    ReadOnlySpan<byte> remaining = source.Slice(idx);
                    int nextUnescapedSegmentLength = remaining.IndexOf(JsonConstants.BackSlash);
                    if (nextUnescapedSegmentLength < 0)
                    {
                        nextUnescapedSegmentLength = remaining.Length;
                    }

                    if ((uint)(written + nextUnescapedSegmentLength) >= (uint)destination.Length)
                    {
                        goto DestinationTooShort;
                    }

                    switch (nextUnescapedSegmentLength)
                    {
                        case 1:
                            destination[written++] = source[idx++];
                            break;
                        case 2:
                            destination[written++] = source[idx++];
                            destination[written++] = source[idx++];
                            break;
                        case 3:
                            destination[written++] = source[idx++];
                            destination[written++] = source[idx++];
                            destination[written++] = source[idx++];
                            break;
                        default:
                            remaining.Slice(0, nextUnescapedSegmentLength).CopyTo(destination.Slice(written));
                            written += nextUnescapedSegmentLength;
                            idx += nextUnescapedSegmentLength;
                            break;
                    }

                    if (idx == source.Length)
                    {
                        goto Success;
                    }
                }
            }

        Success:
            return true;

        DestinationTooShort:
            return false;
        }

        private static bool IsValidUnicodeScalar(uint value)
        {
            // By XORing the incoming value with 0xD800, surrogate code points
            // are moved to the range [ U+0000..U+07FF ], and all valid scalar
            // values are clustered into the single range [ U+0800..U+10FFFF ],
            // which allows performing a single fast range check.

            return IsInRangeInclusive(value ^ 0xD800U, 0x800U, 0x10FFFFU);
        }

        private static bool IsInRangeInclusive(uint value, uint lowerBound, uint upperBound)
          => (value - lowerBound) <= (upperBound - lowerBound);
    }
}
