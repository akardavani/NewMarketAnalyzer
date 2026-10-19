using MarketAnalyzer.Abstractions.ScheduledTasks;
using Microsoft.Extensions.DependencyInjection;

namespace MarketIntelligence.Collectors.ScheduledTasks
{
    internal class CoreWeeklyCandleScheduledTask : DayOfWeekScheduledTaskBase
    {
        public CoreWeeklyCandleScheduledTask(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override DayOfWeek[] DefaultDays => new[] { DayOfWeek.Friday };

        protected override ScheduledTask CreateConfig()
        {
            var task = ScheduledTask.Create(
                 id: TaskId,
                 period: TimeSpan.FromDays(6),
                 startTime: GetStartTime(new TimeOnly(18, 0, 0)),
                 endTime: new TimeOnly(23, 59, 59),
                 description: "حذف و دریافت مجدد کندل‌های هفتگی بازار تهران");

            return task;
        }

        protected override async Task ExecuteCoreAsync(CancellationToken cancellation = default)
        {
            using var scope = ServiceProvider.CreateScope();
            var collector = scope.ServiceProvider.GetRequiredService<CoreCandleCollector>();
            await collector.FetchWeeklyCandles();
        }
    }
}
