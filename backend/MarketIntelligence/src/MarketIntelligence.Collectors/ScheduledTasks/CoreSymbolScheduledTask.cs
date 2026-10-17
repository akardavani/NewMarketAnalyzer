using MarketAnalyzer.Abstractions.ScheduledTasks;
using Microsoft.Extensions.DependencyInjection;

namespace MarketIntelligence.Collectors.ScheduledTasks
{
    internal class CoreSymbolScheduledTask : DayOfWeekScheduledTaskBase
    {
        public CoreSymbolScheduledTask(IServiceProvider serviceProvider) : base(serviceProvider)
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
                 description: "Ø¯Ø±ÛŒØ§ÙØª Ù‡ÙØªÚ¯ÛŒ Ù†Ù…Ø§Ø¯Ù‡Ø§ÛŒ Ù‡Ù…Ù‡ Ø¨Ø§Ø²Ø§Ø±Ù‡Ø§");

            return task;
        }

        protected override async Task ExecuteCoreAsync(CancellationToken cancellation = default)
        {
            using var scope = ServiceProvider.CreateScope();
            var collector = scope.ServiceProvider.GetRequiredService<CoreSymbolCollector>();
            await collector.FetchSymbols();
        }
    }
}
