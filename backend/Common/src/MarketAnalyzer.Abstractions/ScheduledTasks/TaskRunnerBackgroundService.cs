using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace MarketAnalyzer.Abstractions.ScheduledTasks
{
    internal class TaskRunnerBackgroundService : BackgroundService
    {
        private Timer _timer;
        private CancellationToken _cancellation;
        private readonly IConfiguration _configuration;
        private readonly IEnumerable<IScheduledTask> _scheduledTasks;

        public TaskRunnerBackgroundService(IEnumerable<IScheduledTask> scheduledTasks, IConfiguration configuration)
        {
            _scheduledTasks = scheduledTasks;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _cancellation = stoppingToken;
            await Task.WhenAll(_scheduledTasks.Select(t => t.InitAsync(stoppingToken)));
            _timer = new Timer(
                callback: OnTimerTick,
                state: null,
                dueTime: TimeSpan.FromSeconds(1),
                period: TimeSpan.FromSeconds(1));
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();
            _timer = null;
            return base.StartAsync(cancellationToken);
        }

        private void OnTimerTick(object state)
        {
            if (!_configuration.GetValue("DisableTasks", false))
            {
                ExecuteTasksAsync(_cancellation).GetAwaiter().GetResult();
            }
        }

        private Task ExecuteTasksAsync(CancellationToken cancellation)
        {
            var time = DateTime.Now.ToTimeOnly();
            var tasks = _scheduledTasks
                .Where(task => task.IsRuntime(time))
                .Select(task => ExecuteTaskAsync(task, cancellation));

            return Task.WhenAll(tasks);
        }

        private static async Task ExecuteTaskAsync(IScheduledTask task, CancellationToken cancellation)
        {
            try
            {
                await task.ExecuteAsync(cancellation);
            }
            catch (Exception)
            {
                //TODO: Log errors
            }
        }
    }
}
