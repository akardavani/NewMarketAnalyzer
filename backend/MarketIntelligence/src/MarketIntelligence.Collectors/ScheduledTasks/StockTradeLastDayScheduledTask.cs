using MarketAnalyzer.Abstractions.ScheduledTasks;
using Microsoft.Extensions.DependencyInjection;

namespace MarketIntelligence.Collectors.ScheduledTasks
{
    internal class StockTradeLastDayScheduledTask : ScheduledTaskBase
    {
        public StockTradeLastDayScheduledTask(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override ScheduledTask CreateConfig()
        {
            var task = ScheduledTask.Create(
                 id: TaskId,
                 period: TimeSpan.FromHours(23),
                 startTime: new TimeOnly(22, 0, 0),
                 endTime: new TimeOnly(23, 59, 59),
                 description: "Ø¯Ø±ÛŒØ§ÙØª Ù…Ø¹Ø§Ù…Ù„Ø§Øª Ø¢Ø®Ø±ÛŒÙ† Ø±ÙˆØ² Ø³Ù‡Ø§Ù…");

            return task;
        }

        protected override async Task ExecuteCoreAsync(CancellationToken cancellation = default)
        {
            using var scope = ServiceProvider.CreateScope();
            var collector = scope.ServiceProvider.GetRequiredService<StockCollector>();
            await collector.FetchTradeLastDay();
        }
    }
}
