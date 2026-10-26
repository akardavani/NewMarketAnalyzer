using System.Globalization;
using System.Xml.Linq;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// خواندن امن مقدار از XElementهای سرویس TSETMC. کد قدیمی با cast مستقیم (مثل <c>(long)el.Element("x")</c>)
    /// در نبود یک فیلد کل لیست را از بین می‌برد؛ اینجا فیلد ناموجود null می‌شود و ردیف تصمیم می‌گیرد.
    /// </summary>
    internal static class XmlValue
    {
        public static string? Str(XElement row, string name) => ((string?)row.Element(name))?.Trim();

        public static long? Long(XElement row, string name)
            => long.TryParse(Str(row, name), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : null;

        public static int? Int(XElement row, string name)
            => int.TryParse(Str(row, name), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : null;

        public static decimal? Dec(XElement row, string name)
            => decimal.TryParse(Str(row, name), NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : null;

        /// <summary>یکسان‌سازی «ي» و «ك» عربی به «ی» و «ک» فارسی (همان NormalizeArabicName قدیمی).</summary>
        public static string? Persian(string? value)
            => value?.Replace('\u064A', '\u06CC').Replace('\u0643', '\u06A9').Trim();
    }
}
