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
    }
}
