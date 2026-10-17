using System.Globalization;

namespace MarketIntelligence.Infrastructure
{
    public static class PersianDateHelper
    {
        public static DateTime ParseShamsi(string persianDate)
        {
            var parts = persianDate.Split('/');

            var year = int.Parse(parts[0]);
            var month = int.Parse(parts[1]);
            var day = int.Parse(parts[2]);

            var pc = new PersianCalendar();

            return pc.ToDateTime(year, month, day, 0, 0, 0, 0);
        }

        public static DateTime ToGregorian(string shamsi)
        {
            var parts = shamsi.Split('/', '-');

            var year = int.Parse(parts[0]);
            var month = int.Parse(parts[1]);
            var day = int.Parse(parts[2]);

            var pc = new PersianCalendar();
            return pc.ToDateTime(year, month, day, 0, 0, 0, 0);
        }
    }

}
