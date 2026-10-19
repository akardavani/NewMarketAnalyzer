using MarketAnalyzer.Abstractions.ScheduledTasks;
using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer.MarketIntelligence.Collectors.ScheduledTasks
{
    //internal class CoreSymbolScheduledTask : DayOfWeekScheduledTaskBase
    //{
    //    public CoreSymbolScheduledTask(IServiceProvider serviceProvider) : base(serviceProvider)
    //    {
    //    }

    //    protected override DayOfWeek[] DefaultDays => new[] { DayOfWeek.Friday };

    //    protected override ScheduledTask CreateConfig()
    //    {
    //        var task = ScheduledTask.Create(
    //             id: TaskId,
    //             period: TimeSpan.FromDays(6),
    //             //period: TimeSpan.FromMinutes(1),
    //             startTime: GetStartTime(new TimeOnly(18, 0, 0)),
    //             //startTime: new TimeOnly(0, 0, 0),
    //             endTime: new TimeOnly(23, 59, 59),
    //             description: "دریافت هفتگی نمادهای همه بازارها");

    //        return task;
    //    }

    //    protected override async Task ExecuteCoreAsync(CancellationToken cancellation = default)
    //    {
    //        using var scope = ServiceProvider.CreateScope();
    //        var collector = scope.ServiceProvider.GetRequiredService<CoreSymbolCollector>();
    //        await collector.FetchSymbols();
    //    }
    //}

    internal class CoreSymbolScheduledTask : ScheduledTaskBase
    {
        public CoreSymbolScheduledTask(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override ScheduledTask CreateConfig()
        {
            var task = ScheduledTask.Create(
                 id: TaskId,
                 period: TimeSpan.FromHours(12),
                 //period: TimeSpan.FromMinutes(1),
                 startTime: new TimeOnly(0, 0, 0),
                 endTime: new TimeOnly(23, 59, 59),
                 description: "دریافت هفتگی نمادهای همه بازارها");

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
