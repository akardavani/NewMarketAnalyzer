using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>
    /// پیاده‌سازی تقویم بازار.
    /// <list type="bullet">
    /// <item><description>روزانه: نیمه‌شب محلی تا نیمه‌شب محلی بعد.</description></item>
    /// <item><description>هفتگی: برای همهٔ بازارها از دوشنبه (ISO) تا یکشنبه، تا بازارها قابل‌مقایسه باشند.</description></item>
    /// <item><description>ماهانه: برای همهٔ بازارها ماه میلادی.</description></item>
    /// <item><description>زیر روزانه (۴س، ۱س، ۱۵د، ۵د): کف‌گیری ساده روی UTC.</description></item>
    /// </list>
    /// نکتهٔ DST: اگر نیمه‌شب محلی در یک منطقه وجود نداشته باشد (جهش ساعت)، یک ساعت جلو می‌رود.
    /// </summary>
    public sealed class MarketTradingCalendar : ITradingCalendar
    {
        /// <summary>شروع هفته برای همهٔ بازارها دوشنبه است (یک‌دست بودن تقویم بین بازارها، تصمیم محصول).</summary>
        private const DayOfWeek WeekStart = DayOfWeek.Monday;

        /// <summary>سازنده.</summary>
        public MarketTradingCalendar(TradingCalendarKind kind, TimeZoneInfo timeZone)
        {
            Kind = kind;
            TimeZone = timeZone;
        }

        /// <inheritdoc />
        public TradingCalendarKind Kind { get; }

        /// <inheritdoc />
        public TimeZoneInfo TimeZone { get; }

        /// <inheritdoc />
        public DateTime AlignToPeriodStart(DateTime utc, Timeframe timeframe)
        {
            utc = EnsureUtc(utc);

            if (TryGetIntradayStep(timeframe, out var step))
                return new DateTime(utc.Ticks - utc.Ticks % step.Ticks, DateTimeKind.Utc);

            var local = TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZone);
            return LocalToUtc(LocalPeriodStart(local, timeframe));
        }

        /// <inheritdoc />
        public DateTime GetPeriodEndUtc(DateTime periodStartUtc, Timeframe timeframe)
        {
            periodStartUtc = EnsureUtc(periodStartUtc);

            if (TryGetIntradayStep(timeframe, out var step))
                return periodStartUtc + step;

            var local = TimeZoneInfo.ConvertTimeFromUtc(periodStartUtc, TimeZone);
            var localStart = LocalPeriodStart(local, timeframe);
            return LocalToUtc(NextLocalPeriodStart(localStart, timeframe));
        }

        /// <inheritdoc />
        public bool IsPeriodClosed(DateTime periodStartUtc, Timeframe timeframe, DateTime nowUtc)
            => GetPeriodEndUtc(periodStartUtc, timeframe) <= EnsureUtc(nowUtc);

        /// <inheritdoc />
        public DateOnly GetLocalDate(DateTime utc)
            => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(EnsureUtc(utc), TimeZone));

        private DateTime LocalPeriodStart(DateTime local, Timeframe timeframe)
        {
            switch (timeframe)
            {
                case Timeframe.Daily:
                    return local.Date;

                case Timeframe.Weekly:
                    var back = ((int)local.DayOfWeek - (int)WeekStart + 7) % 7;
                    return local.Date.AddDays(-back);

                case Timeframe.Mountly:
                    return new DateTime(local.Year, local.Month, 1);

                default:
                    throw new NotSupportedException($"Timeframe {timeframe} is not supported by calendar-based alignment.");
            }
        }

        private DateTime NextLocalPeriodStart(DateTime localStart, Timeframe timeframe)
        {
            switch (timeframe)
            {
                case Timeframe.Daily:
                    return localStart.AddDays(1);

                case Timeframe.Weekly:
                    return localStart.AddDays(7);

                case Timeframe.Mountly:
                    return localStart.AddMonths(1);

                default:
                    throw new NotSupportedException($"Timeframe {timeframe} is not supported by calendar-based alignment.");
            }
        }

        private DateTime LocalToUtc(DateTime local)
        {
            var unspecified = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);

            // Problem: در منطقه‌هایی با DST ممکن است ساعت 00:00 محلی در روز جهش وجود نداشته باشد و
            // ConvertTimeToUtc استثنا بدهد. Fix: یک ساعت جلو می‌رویم تا به اولین لحظهٔ معتبر همان روز برسیم.
            if (TimeZone.IsInvalidTime(unspecified))
                unspecified = unspecified.AddHours(1);

            return TimeZoneInfo.ConvertTimeToUtc(unspecified, TimeZone);
        }

        private static bool TryGetIntradayStep(Timeframe timeframe, out TimeSpan step)
        {
            switch (timeframe)
            {
                case Timeframe.FourHours: step = TimeSpan.FromHours(4); return true;
                case Timeframe.OneHour: step = TimeSpan.FromHours(1); return true;
                case Timeframe.FifteenMinutes: step = TimeSpan.FromMinutes(15); return true;
                case Timeframe.FiveMinutes: step = TimeSpan.FromMinutes(5); return true;
                default: step = TimeSpan.Zero; return false;
            }
        }

        private static DateTime EnsureUtc(DateTime value) => value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
