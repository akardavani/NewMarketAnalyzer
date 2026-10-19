using MarketAnalyzer.Abstractions.ScheduledTasks;
using Microsoft.Extensions.DependencyInjection;

namespace MarketIntelligence.Collectors.ScheduledTasks
{
    internal class CoreCandleScheduledTask : ScheduledTaskBase
    {
        public CoreCandleScheduledTask(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override ScheduledTask CreateConfig()
        {
            var task = ScheduledTask.Create(
                 id: TaskId,
                 //period: TimeSpan.FromHours(12),
                 period: TimeSpan.FromHours(2),
                 //period: TimeSpan.FromMinutes(32),
                 startTime: new TimeOnly(0, 0, 0),
                 endTime: new TimeOnly(23, 59, 59),
                 description: "دریافت کندل‌های روزانه همه بازارها");

            return task;
        }

        protected override async Task ExecuteCoreAsync(CancellationToken cancellation = default)
        {
            using var scope = ServiceProvider.CreateScope();
            var collector = scope.ServiceProvider.GetRequiredService<CoreCandleCollector>();
            await collector.FetchCandles();
        }
    }
}
