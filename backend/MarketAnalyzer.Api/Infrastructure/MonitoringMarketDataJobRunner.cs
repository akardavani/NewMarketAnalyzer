using System;
using System.Threading;
using System.Threading.Tasks;
using MarketAnalyzer.DataMonitoring.Application;
using MarketAnalyzer.MarketData.Application.Operations;
using MarketAnalyzer.MarketData.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.Api.Infrastructure;

public sealed class MonitoringMarketDataJobRunner : IMarketDataJobRunner
{
    private static readonly TimeSpan MonitoringTimeout = TimeSpan.FromSeconds(2);

    private readonly MarketDataJobRunner _inner;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MonitoringMarketDataJobRunner> _logger;
    private int _monitoringAvailable = 1;

    public MonitoringMarketDataJobRunner(MarketDataJobRunner inner, IServiceScopeFactory scopeFactory, ILogger<MonitoringMarketDataJobRunner> logger)
    {
        _inner = inner;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<MarketJobResult> RunAsync(string marketKey, MarketJobKind job, CancellationToken cancellationToken, MarketJobTrigger trigger = MarketJobTrigger.Manual)
    {
        Guid? runId = null;
        if (Volatile.Read(ref _monitoringAvailable) != 0)
        {
            try
            {
                using var timeout = new CancellationTokenSource(MonitoringTimeout);
                using var scope = _scopeFactory.CreateScope();
                runId = await scope.ServiceProvider.GetRequiredService<IDataMonitoringService>()
                    .BeginRunAsync(new DataJobRunStart("market-data", marketKey, job.ToString(), trigger.ToString(), null), timeout.Token);
            }
            catch (Exception ex)
            {
                DisableMonitoring(ex, "start");
            }
        }

        try
        {
            var result = await _inner.RunAsync(marketKey, job, cancellationToken, trigger);
            if (runId.HasValue)
            {
                var status = result.Status switch
                {
                    MarketJobStatus.Skipped => "Skipped",
                    MarketJobStatus.Failed => "Failed",
                    _ when result.Failed > 0 => "Partial",
                    _ => "Completed"
                };
                await TryFinishAsync(runId.Value, new DataJobRunFinish(status, result.Processed, result.Succeeded,
                    result.Updated, result.NoData, result.Failed, result.Message, result.ErrorSamples));
            }
            return result;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            if (runId.HasValue)
                await TryFinishAsync(runId.Value, new DataJobRunFinish("Cancelled", 0, 0, 0, 0, 0, "Execution was cancelled.", Array.Empty<string>()));
            throw;
        }
        catch (Exception ex)
        {
            if (runId.HasValue)
                await TryFinishAsync(runId.Value, new DataJobRunFinish("Failed", 0, 0, 0, 0, 1, ex.Message, new[] { ex.Message }));
            throw;
        }
    }

    private async Task TryFinishAsync(Guid runId, DataJobRunFinish finish)
    {
        try
        {
            using var timeout = new CancellationTokenSource(MonitoringTimeout);
            using var scope = _scopeFactory.CreateScope();
            await scope.ServiceProvider.GetRequiredService<IDataMonitoringService>().FinishRunAsync(runId, finish, timeout.Token);
        }
        catch (Exception ex)
        {
            DisableMonitoring(ex, "finish");
        }
    }

    private void DisableMonitoring(Exception exception, string operation)
    {
        if (Interlocked.Exchange(ref _monitoringAvailable, 0) != 0)
            _logger.LogWarning(exception,
                "Data monitoring persistence failed during {Operation}; monitoring is paused until the API restarts. Market data jobs will continue.",
                operation);
    }
}
