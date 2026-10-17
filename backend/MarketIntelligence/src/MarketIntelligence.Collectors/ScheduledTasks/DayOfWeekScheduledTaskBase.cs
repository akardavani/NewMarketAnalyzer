using MarketAnalyzer.Abstractions.ScheduledTasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketIntelligence.Collectors.ScheduledTasks
{
    /// <summary>
    /// ØªØ³Ú© Ø²Ù…Ø§Ù†â€ŒØ¨Ù†Ø¯ÛŒâ€ŒØ´Ø¯Ù‡â€ŒØ§ÛŒ Ú©Ù‡ ÙÙ‚Ø· Ø¯Ø± Ø±ÙˆØ²Ù‡Ø§ÛŒ Ù…Ø´Ø®ØµÛŒ Ø§Ø² Ù‡ÙØªÙ‡ Ø§Ø¬Ø±Ø§ Ù…ÛŒâ€ŒØ´ÙˆØ¯.
    /// Ø±ÙˆØ²Ù‡Ø§ Ø§Ø² appsettings: CollectorSchedule:{TaskId}:Days  (Ù…Ø«Ù„Ø§Ù‹ ["Friday"])
    /// Ø¨Ø¯ÙˆÙ† ØªØºÛŒÛŒØ± Ø¯Ø± ScheduledTaskBase Ù…Ø´ØªØ±Ú©.
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

        /// <summary>Ø±ÙˆØ²Ù‡Ø§ÛŒ Ù¾ÛŒØ´â€ŒÙØ±Ø¶ Ø¯Ø± ØµÙˆØ±Øª Ù†Ø¨ÙˆØ¯ ØªÙ†Ø¸ÛŒÙ…Ø§Øª.</summary>
        protected abstract DayOfWeek[] DefaultDays { get; }

        /// <summary>Ø³Ø§Ø¹Øª Ø´Ø±ÙˆØ¹ Ø§Ø² ØªÙ†Ø¸ÛŒÙ…Ø§Øª ÛŒØ§ Ù…Ù‚Ø¯Ø§Ø± Ù¾ÛŒØ´â€ŒÙØ±Ø¶.</summary>
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
