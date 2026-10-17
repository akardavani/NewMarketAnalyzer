using System;
using System.Globalization;
using System.Text;

namespace MarketAnalyzer.Mmtp
{
    public static class MmtpConvert
    {
        public const string DateTimeFormat8 = "yyyyMMdd";
        public const string DateTimeFormat14 = "yyyyMMddHHmmss";
        public const string DateTimeFormat20 = "yyyyMMddHHmmssffffff";
        public static readonly Encoding Encoding = CodePagesEncodingProvider.Instance.GetEncoding(1256);

        public static string GetString(byte[] buffer, int offset, int length)
        {
            var value = Encoding.GetString(buffer, offset, length)?.Trim();
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return value.Replace("ي", "ی").Replace("ك", "ک").Trim();
        }

        public static DateTime GetDateTime(byte[] buffer, int offset, int length)
        {
            var value = GetNullableDateTime(buffer, offset, length);
            return value.Value;
        }

        public static DateTime? GetNullableDateTime(byte[] buffer, int offset, int length)
        {
            var value = GetString(buffer, offset, length);
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var format = length switch
            {
                8 => DateTimeFormat8,
                14 => DateTimeFormat14,
                20 => DateTimeFormat20,
                _ => throw new FormatException($"Invalid date time length: {length}")
            };
            return DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
        }

        public static int GetInt32(byte[] buffer, int offset, int length)
        {
            var value = GetString(buffer, offset, length);
            return string.IsNullOrEmpty(value) ?
                0 :
                int.Parse(value, CultureInfo.InvariantCulture);
        }

        public static long GetInt64(byte[] buffer, int offset, int length)
        {
            var value = GetString(buffer, offset, length);
            return string.IsNullOrEmpty(value) ?
                0L :
                long.Parse(value, CultureInfo.InvariantCulture);
        }

        public static bool GetBoolean(byte[] buffer, int offset)
        {
            var value = GetString(buffer, offset, 1);
            return value switch
            {
                "0" => true,
                "1" => false,
                _ => throw new NotSupportedException("Invalid value")
            };
        }

        public static decimal GetDecimal(byte[] buffer, int offset, int length)
        {
            var value = GetString(buffer, offset, length);
            if (string.IsNullOrWhiteSpace(value))
            {
                return default;
            }

            var firstChar = value[0];
            if (firstChar == ' ')
            {
                firstChar = '0';
            }

            bool positive;
            int numberOfDecimals;
            if (char.IsDigit(firstChar))
            {
                positive = true;
                numberOfDecimals = firstChar - '0';
            }
            else
            {
                positive = false;
                numberOfDecimals = firstChar - 'A';
            }

            if (numberOfDecimals > 0)
            {
                value = value.Insert(value.Length - numberOfDecimals, ".");
            }
            value = value[1..];
            var nmb = decimal.Parse(value, CultureInfo.InvariantCulture);
            return positive ? nmb : -nmb;
        }

        public static TradeCancellationFlag GetTradeCancellationFlag(byte[] buffer, int offset)
        {
            var value = GetString(buffer, offset, 2);
            return value switch
            {
                "00" => TradeCancellationFlag.TradeCancellation,
                "07" => TradeCancellationFlag.Trade,
                _ => throw new NotSupportedException("Invalid TradeCancellationFlag"),
            };
        }
    }
}