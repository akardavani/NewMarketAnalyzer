using System.Globalization;
using System.Linq;

namespace System
{
    public static class DateTimeExtensions
    {
        private static readonly CultureInfo _faCulture = new("fa-IR");
        private static readonly PersianCalendar _pc = new();

        public static string ToPersian(this DateTime date, string format = "yyyy/MM/dd")
        {
            return date.ToString(format, _faCulture);
        }

        public static string ToPersian(this DateOnly date, string format = "yyyy/MM/dd")
        {
            return date.ToString(format, _faCulture);
        }
        public static DateOnly? FromPersian(this string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            var normalized = new string(s.Trim().Select(c =>
            c is >= '۰' and <= '۹' ? (char)('0' + (c - '۰')) :
            c is >= '٠' and <= '٩' ? (char)('0' + (c - '٠')) : c).ToArray());

            var parts = normalized.Split('/', '-');
            if (parts.Length != 3 ||
                !int.TryParse(parts[0], out var y) ||
                !int.TryParse(parts[1], out var m) ||
                !int.TryParse(parts[2], out var d))
                return null;

            try { return DateOnly.FromDateTime(_pc.ToDateTime(y, m, d, 0, 0, 0, 0)); }
            catch { return null; }
        }
    }
}

