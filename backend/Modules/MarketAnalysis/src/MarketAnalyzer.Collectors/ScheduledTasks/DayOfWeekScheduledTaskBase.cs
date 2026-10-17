using MarketAnalyzer.Abstractions.ScheduledTasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer.Collectors.ScheduledTasks
{
    /// <summary>
    /// تسک زمان‌بندی‌شده‌ای که فقط در روزهای مشخصی از هفته اجرا می‌شود.
    /// روزها از appsettings: CollectorSchedule:{TaskId}:Days  (مثلاً ["Friday"])
    /// بدون تغییر در ScheduledTaskBase مشترک.
    /// </summary>
    internal abstract class DayOfWeekScheduledTaskBase : ScheduledTaskBase
    {
        private readonly HashSet<DayOfWeek> _days;

        protected DayOfWeekScheduledTaskBase(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var configured = configuration.GetSection($"CollectorSchedule:{TaskId}:Days").Get<DayOfWeek[]>();
            _days = new HashSet<DayOfWeek>(configured is { Length: > 0 } ? configured : DefaultDays);
        }

        /// <summary>روزهای پیش‌فرض در صورت نبود تنظیمات.</summary>
        protected abstract DayOfWeek[] DefaultDays { get; }

        /// <summary>ساعت شروع از تنظیمات یا مقدار پیش‌فرض.</summary>
        protected TimeOnly GetStartTime(TimeOnly defaultValue)
        {
            var configuration = ServiceProvider.GetRequiredService<IConfiguration>();
            var value = configuration[$"CollectorSchedule:{TaskId}:StartTime"];
            return TimeOnly.TryParse(value, out var t) ? t : defaultValue;
        }

        public override bool IsRuntime(TimeOnly time)
            => _days.Contains(DateTime.Now.DayOfWeek) && base.IsRuntime(time);
    }
}
