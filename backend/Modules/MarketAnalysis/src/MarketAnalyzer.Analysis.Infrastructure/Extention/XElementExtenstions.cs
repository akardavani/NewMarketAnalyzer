using System.Globalization;
using System.Xml.Linq;

namespace System
{
    internal static class XElementExtenstions
    {
        private const string _dateFormat = "yyyyMMdd";
        private const string _dateTimeFormat = "yyyyMMddHHmmss";

        public static DateOnly GetDateOnly(this XElement element, string dateProperty)
        {
            var value = element.Element(dateProperty).Value;
            return DateOnly.ParseExact(value.Trim(), _dateFormat, CultureInfo.InvariantCulture);
        }

        public static DateOnly? GetNullableDateOnly(this XElement element, string dateProperty)
        {
            var value = element.Element(dateProperty)?.Value?.Trim();
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return DateOnly.ParseExact(value, _dateFormat, CultureInfo.InvariantCulture);
        }

        public static DateTime GetDateTime(this XElement element, string dateProperty, string timeProperty = null)
        {
            var value = element.Element(dateProperty).Value?.Trim();
            if (!string.IsNullOrEmpty(timeProperty))
            {
                value += element.Element(timeProperty).Value.PadLeft(6, '0');
            }

            var format = value.Length switch
            {
                14 => _dateTimeFormat,
                8 => _dateFormat,
                _ => throw new FormatException($"Invalid DateTime format({value})."),
            };
            return DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
        }

        public static TimeSpan GetTime(this XElement element, string property)
        {
            var time = element.Element(property).Value.PadLeft(6, '0');
            return new TimeSpan(
                int.Parse(time[..2]),
                int.Parse(time.Substring(2, 2)),
                int.Parse(time.Substring(4, 2))
            );
        }

        public static int GetInt32(this XElement element, string property)
        {
            return int.Parse(element.Element(property).Value, CultureInfo.InvariantCulture);
        }

        public static int? GetNullableInt32(this XElement element, string property)
        {
            var value = element.Element(property)?.Value?.Trim();
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            return int.Parse(value, CultureInfo.InvariantCulture);
        }

        public static long GetInt64(this XElement element, string property)
        {
            return long.Parse(element.Element(property).Value, CultureInfo.InvariantCulture);
        }

        public static long? GetNullableInt64(this XElement element, string property)
        {
            var value = element.Element(property)?.Value?.Trim();
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            return long.Parse(value, CultureInfo.InvariantCulture);
        }

        public static string GetString(this XElement element, string property)
        {
            var value = element.Element(property).Value;
            return string.IsNullOrEmpty(value) ?
                value :
                value.Replace("ي", "ی").Replace("ك", "ک");
        }

        public static decimal GetDecimal(this XElement element, string property)
        {
            return decimal.Parse(element.Element(property).Value, CultureInfo.InvariantCulture);
        }

        public static byte GetByte(this XElement element, string property)
        {
            return byte.Parse(element.Element(property).Value, CultureInfo.InvariantCulture);
        }

        public static bool GetBoolean(this XElement element, string property)
        {
            var value = element.Element(property).Value;
            return value switch
            {
                "0" or "false" => false,
                "1" or "true" => true,
                _ => throw new FormatException($"Invalid boolean value, property name: [{property}] and value: [{value}].")
            };
        }

    }
}
