using System;
using System.Threading.Tasks;
using System.Threading;

namespace MarketAnalyzer.Abstractions.ScheduledTasks
{
    public interface IScheduledTask
    {
        bool IsRuntime(TimeOnly time);
        Task InitAsync(CancellationToken cancellation = default);
        Task ExecuteAsync(CancellationToken cancellation = default);
    }
}
