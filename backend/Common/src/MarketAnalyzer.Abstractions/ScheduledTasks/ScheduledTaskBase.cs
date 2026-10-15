using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.Abstractions.ScheduledTasks
{
    public abstract class ScheduledTaskBase : IScheduledTask
    {
        private bool _isRunning;
        private ScheduledTask _config;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public ScheduledTaskBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = serviceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType());
        }

        protected string TaskId => GetType().Name
            .Replace("ProviderScheduledTask", "")
            .Replace("ScheduledTask", "");

        protected IServiceProvider ServiceProvider => _serviceProvider;

        public virtual bool IsRuntime(TimeOnly time)
        {
            return !_isRunning &&
                !_config.Disabled &&
                time >= _config.StartTime &&
                time <= _config.EndTime &&
                (_config.LastRunSucceed == false ||
                 _config.LastRunTime is null ||
                 DateTime.Now.Subtract(_config.LastRunTime.Value) > _config.Period);
        }

        public virtual async Task InitAsync(CancellationToken cancellation = default)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
            var currentConfig = CreateConfig() ?? throw new Exception($"The scheduled task `{TaskId}` is not configured.");
            _config = await dbContext
                .Set<ScheduledTask>()
                .FirstOrDefaultAsync(e => e.Id == TaskId, cancellation);

            if (_config is null)
            {
                _config = currentConfig;
                dbContext.Set<ScheduledTask>().Add(_config);
            }
            else
            {
                _config.Update(
                    period: currentConfig.Period,
                    startTime: currentConfig.StartTime,
                    endTime: currentConfig.EndTime,
                    description: currentConfig.Description);
                _config.Disable(currentConfig.Disabled);
            }

            await dbContext.SaveChangesAsync(cancellation);
        }

        public virtual async Task ExecuteAsync(CancellationToken cancellation = default)
        {
            var succeed = false;
            var startTime = DateTime.Now;
            _isRunning = true;
            try
            {
                await ExecuteCoreAsync(cancellation);
                succeed = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error on excecute: {TaskName}.ExecuteAsync(...)", GetType().Name);
            }
            finally
            {
                await UpdateLastRunAsync(startTime: startTime, succeed: succeed, cancellation: cancellation);
                _isRunning = false;
            }
        }

        protected abstract ScheduledTask CreateConfig();
        protected abstract Task ExecuteCoreAsync(CancellationToken cancellation = default);

        private async Task UpdateLastRunAsync(DateTime startTime, bool succeed, CancellationToken cancellation)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
                dbContext.TryAttach(_config);
                _config.UpdateLastRun(lastRunTime: startTime, lastRunSucceed: succeed);
                await dbContext.SaveChangesAsync(cancellation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error on excecute: ScheduledTaskBase.UpdateLastRunAsync(...)");
            }
        }
    }
}
