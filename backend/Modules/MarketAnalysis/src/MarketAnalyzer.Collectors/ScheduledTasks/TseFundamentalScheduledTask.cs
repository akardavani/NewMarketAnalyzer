using MarketAnalyzer.Abstractions.ScheduledTasks;
using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer.Collectors.ScheduledTasks
{
    internal class TseFundamentalScheduledTask : ScheduledTaskBase
    {
        public TseFundamentalScheduledTask(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override ScheduledTask CreateConfig()
        {
            var task = ScheduledTask.Create(
                 id: TaskId,
                 period: TimeSpan.FromHours(23),
                 startTime: new TimeOnly(22, 0, 0),
                 endTime: new TimeOnly(23, 59, 59),
                 description: "دریافت گزارش‌های مالی (بنیادی) بورس تهران");

            return task;
        }

        protected override async Task ExecuteCoreAsync(CancellationToken cancellation = default)
        {
            using var scope = ServiceProvider.CreateScope();
            var collector = scope.ServiceProvider.GetRequiredService<TseFundamentalCollector>();
            await collector.FetchData();
        }
    }
}
