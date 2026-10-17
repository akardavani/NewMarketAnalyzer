using MarketAnalyzer.Abstractions.ScheduledTasks;
using Microsoft.Extensions.DependencyInjection;

namespace MarketIntelligence.Collectors.ScheduledTasks
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
                 description: "Ø¯Ø±ÛŒØ§ÙØª Ú¯Ø²Ø§Ø±Ø´â€ŒÙ‡Ø§ÛŒ Ù…Ø§Ù„ÛŒ (Ø¨Ù†ÛŒØ§Ø¯ÛŒ) Ø¨ÙˆØ±Ø³ ØªÙ‡Ø±Ø§Ù†");

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
