using MarketAnalyzer.Abstractions.ScheduledTasks;
using Microsoft.Extensions.DependencyInjection;

namespace MarketIntelligence.Collectors.ScheduledTasks
{
    internal class InvestmentFundChartScheduledTask : ScheduledTaskBase
    {
        public InvestmentFundChartScheduledTask(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override ScheduledTask CreateConfig()
        {
            var task = ScheduledTask.Create(
                 id: TaskId,
                 period: TimeSpan.FromHours(23),
                 startTime: new TimeOnly(22, 0, 0),
                 endTime: new TimeOnly(23, 59, 59),
                 description: "Ø¯Ø±ÛŒØ§ÙØª Ù†Ù…ÙˆØ¯Ø§Ø± ØµÙ†Ø¯ÙˆÙ‚â€ŒÙ‡Ø§ÛŒ Ø³Ø±Ù…Ø§ÛŒÙ‡â€ŒÚ¯Ø°Ø§Ø±ÛŒ");

            return task;
        }

        protected override async Task ExecuteCoreAsync(CancellationToken cancellation = default)
        {
            using var scope = ServiceProvider.CreateScope();
            var collector = scope.ServiceProvider.GetRequiredService<InvestmentFundCollector>();
            await collector.FetchInvestmentFundChart();
        }
    }
}
