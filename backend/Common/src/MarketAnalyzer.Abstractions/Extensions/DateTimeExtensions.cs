using System.Collections.Generic;
using System.Globalization;

namespace System
{
    [Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public static class DateTimeExtensions
    {
        private const string DateFormat = "yyyy-MM-dd";
        public static readonly PersianCalendar Calendar = new();
        public static readonly CultureInfo FaCulture = new("fa-IR");

        //public static readonly PersianCalendar PersianCalendar = new();

        public static string ToPersian(this DateTime dateTime, string format = DateFormat)
        {
            return dateTime.ToString(format, FaCulture);
        }

        public static string ToPersian(this DateOnly date, string format = DateFormat)
        {
            return date.ToString(format, FaCulture);
        }

        public static DateOnly ToDateOnly(this DateTime dateTime)
        {
            return DateOnly.FromDateTime(dateTime);
        }

        public static DateOnly? ToDateOnly(this DateTime? dateTime)
        {
            return dateTime is null ?
                null :
                DateOnly.FromDateTime(dateTime.Value);
        }

        public static TimeOnly ToTimeOnly(this DateTime dateTime)
        {
            return TimeOnly.FromDateTime(dateTime);
        }

        public static TimeOnly? ToTimeOnly(this DateTime? dateTime)
        {
            return dateTime is null ?
                null :
                TimeOnly.FromDateTime(dateTime.Value);
        }

        public static TimeOnly ToTimeOnly(this TimeSpan time)
        {
            return TimeOnly.FromTimeSpan(time);
        }

        public static TimeOnly? ToTimeOnly(this TimeSpan? time)
        {
            return time is null ?
                null :
                TimeOnly.FromTimeSpan(time.Value);
        }

        public static DateOnly IsDefault(this DateOnly date, DateOnly replace)
        {
            return date == new DateOnly(1, 1, 1) ? replace : date;
        }

        public static int GetJalaliYear(this DateOnly date)
        {
            return Calendar.GetYear(date.ToDateTime(TimeOnly.MinValue));
        }

        public static int GetDaysInYear(this DateOnly date)
        {
            return GetDaysInYear(date.ToDateTime(TimeOnly.MinValue));
        }

        public static int GetDaysInYear(this DateTime date)
        {
            var year = Calendar.GetYear(date);
            return Calendar.GetDaysInYear(year);
        }

        public static int GetDaysInMonth(this DateOnly date)
        {
            return GetDaysInMonth(date.ToDateTime(TimeOnly.MinValue));
        }

        public static int GetDaysInMonth(this DateTime date)
        {
            var year = Calendar.GetYear(date);
            var month = Calendar.GetMonth(date);
            return Calendar.GetDaysInMonth(year, month);
        }

        public static IReadOnlyCollection<DateOnly> GetDaysOfYear(this DateTime date)
        {
            var days = new List<DateOnly>(366);
            var (FirstDate, LastDate) = GetFirstAndLastDates(date);
            for (var dt = FirstDate; dt <= LastDate; dt = dt.AddDays(1))
            {
                days.Add(dt);
            }

            return days;
        }

        public static DateOnly GetFirstDateOfYear(this DateTime date)
        {
            var persianYear = Calendar.GetYear(date);
            return new DateOnly(persianYear, 1, 1, Calendar);
        }

        public static DateOnly GetFirstDateOfMonth(this DateTime date)
        {
            var persianYear = Calendar.GetYear(date);
            var persianMonth = Calendar.GetMonth(date);
            return new DateOnly(persianYear, persianMonth, 1, Calendar);
        }

        public static (DateOnly FirstDate, DateOnly LastDate) GetFirstAndLastDates(this DateTime date)
        {
            var persianYear = Calendar.GetYear(date);
            var startDate = new DateOnly(persianYear, 1, 1, Calendar);
            var endDate = new DateOnly(persianYear, 12, Calendar.GetDaysInMonth(persianYear, 12), Calendar);
            return (startDate, endDate);
        }

        /// <summary>
        /// ماه شمسی قبل
        /// </summary>
        /// <param name="date">تاریخ</param>
        /// <param name="isLastDay">اگر روز آخر ماه قبل رو بخواهیم</param>
        /// <param name="isFirstDay">اگر روز اول ماه قبل رو بخواهیم</param>
        public static DateOnly GetPersianPreviousMonth(this DateOnly date, bool isLastDay = false, bool isFirstDay = false)
        {
            var pc = new PersianCalendar();
            var dt = date.ToDateTime(new TimeOnly(0, 0));

            var persianYear = pc.GetYear(dt);
            var persianMonth = pc.GetMonth(dt);

            if (persianMonth == 1)
            {
                persianYear--;
                persianMonth = 12;
            }
            else
            {
                persianMonth--;
            }

            int day;
            switch (true)
            {
                case bool _ when isLastDay:
                    day = pc.GetDaysInMonth(persianYear, persianMonth); // آخرین روز ماه
                    break;
                case bool _ when isFirstDay:
                    day = 1; // اولین روز ماه
                    break;
                default:
                    var persianDay = pc.GetDayOfMonth(dt);
                    day = Math.Min(persianDay, pc.GetDaysInMonth(persianYear, persianMonth)); // روز مشابه ماه قبل
                    break;
            }

            var newDate = pc.ToDateTime(persianYear, persianMonth, day, 0, 0, 0, 0);
            return DateOnly.FromDateTime(newDate);
        }


        public static string DateTimeToUnixTimestamp(this DateTime dateTime)
        {
            var timestamp = (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

            return string.Format("{0}", (long)timestamp);
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        public static DateTime StringToDateTime(string dateStr)
        {
            DateTime date;
            if (DateTime.TryParseExact(dateStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }
            else
            {
                return DateTime.Today;
            }
        }

        public static DateOnly ToGregorian(this string format)
        {
            var input = "1405/03/23";
            var parts = input.Split('/');

            var year = int.Parse(parts[0]);
            var month = int.Parse(parts[1]);
            var day = int.Parse(parts[2]);

            var pc = new PersianCalendar();
            var gregorian = pc.ToDateTime(year, month, day, 0, 0, 0, 0);

            var dateOnly = DateOnly.FromDateTime(gregorian);

            return dateOnly;
        }

        public static DateTime ToDate(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
        }


        public static DateOnly Min(this DateOnly original, DateOnly compare)
        {
            return (original <= compare) ?
                original :
                compare;
        }

        public static DateOnly GetFirstDayOfYear(this DateTime date)
        {
            var persianYear = Calendar.GetYear(date);
            return Calendar.ToDateTime(persianYear, 1, 1, 0, 0, 0, 0).ToDateOnly();
        }

        public static DateOnly ShamsiToMiladi(this string date)
        {
            DateOnly dateOnly = new DateOnly(2025, 1, 1);
            var parts = date.Split('/');
            if (parts.Length == 3)
            {
                int y = int.Parse(parts[0]);
                int m = int.Parse(parts[1]);
                int d = int.Parse(parts[2]);

                dateOnly = Calendar.ToDateTime(y, m, d, 0, 0, 0, 0).ToDateOnly();
            }

            return dateOnly;
        }

        public static bool IsHoliday(this DateOnly date, out string reason)
        {
            return IsHoliday(date.ToDateTime(TimeOnly.MinValue), out reason);
        }

        public static bool IsHoliday(this DateTime date, out string reason)
        {
            var month = Calendar.GetMonth(date);
            var day = Calendar.GetDayOfMonth(date);
            var monthName = FaCulture.DateTimeFormat.GetMonthName(month);

            var isHoliday =
                //تعطیلات فروردین
                (month == 1 && (day == 1 || day == 2 || day == 3 || day == 4 || day == 12 || day == 13)) ||
                // تعطیلات خرداد
                (month == 3 && (day == 14 || day == 15)) ||
                // تعطیلات بهمن
                (month == 11 && (day == 22)) ||
                // تعطیلات اسفند
                (month == 12 && (day == 29 || day == 30));

            if (isHoliday)
            {
                reason = $"تعطیل رسمی {day} {monthName}";
                return true;
            }
            else if (date.DayOfWeek == DayOfWeek.Thursday || date.DayOfWeek == DayOfWeek.Friday)
            {
                reason = $"تعطیل {FaCulture.DateTimeFormat.GetDayName(date.DayOfWeek)} روز غیرکاری";
                return true;
            }
            else
            {
                reason = null;
                return false;
            }
        }

        public static string ConvertNumToTime(long num)
        {
            long hours = num / 10000;      // محاسبه ساعت
            long minutes = (num % 10000) / 100;    // محاسبه دقیقه
            long seconds = num % 100;      // محاسبه ثانیه

            return $"{hours:00}:{minutes:00}:{seconds:00}";
        }
    }
}
