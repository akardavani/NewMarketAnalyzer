using System.Reflection;
using Cronos;
using MarketAnalyzer.Collectors;

// فرض: TimeframeSchedule و CollectorScheduleSettings و IMarketCollector در یک جا تعریف شده‌اند
namespace MarketAnalyzer.CollectorRunner
{
    public class CronJobService
    {
        private readonly IServiceProvider _provider;
        private readonly CollectorScheduleSettings _settings;
        private readonly Dictionary<string, CancellationTokenSource> _jobs = new();

        public CronJobService(IServiceProvider provider, CollectorScheduleSettings settings)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void Start()
        {
            foreach (var collector in _settings.Collectors.Where(x => x.Enabled))
            {
                foreach (var schedule in collector.Timeframes)
                {
                    if (string.IsNullOrWhiteSpace(schedule?.Cron))
                        continue;

                    StartJob(collector.Name, schedule);
                }
            }
        }

        private void StartJob(string collectorName, TimeframeSchedule schedule)
        {
            var key = $"{collectorName}-{schedule.Timeframe}";

            if (_jobs.ContainsKey(key)) return;

            var cts = new CancellationTokenSource();
            _jobs[key] = cts;

            Task.Run(() => RunJobLoop(collectorName, schedule, cts.Token), cts.Token);
        }

        private async Task RunJobLoop(string collectorName, TimeframeSchedule schedule, CancellationToken ct)
        {
            CronExpression cron;
            try
            {
                cron = CronExpression.Parse(schedule.Cron, CronFormat.Standard);
            }
            catch (Exception ex)
            {
                // Log invalid cron and stop this job
                Console.WriteLine($"Invalid cron for {collectorName}-{schedule.Timeframe}: {ex.Message}");
                return;
            }

            while (!ct.IsCancellationRequested)
            {
                var next = cron.GetNextOccurrence(DateTime.UtcNow);
                if (!next.HasValue)
                {
                    // no next occurrence (e.g. invalid expression) -> break
                    break;
                }

                var delay = next.Value - DateTime.UtcNow;
                if (delay > TimeSpan.Zero)
                {
                    try
                    {
                        await Task.Delay(delay, ct);
                    }
                    catch (TaskCanceledException) { break; }
                }

                // create scope per execution
                using var scope = _provider.CreateScope();
                var collectors = scope.ServiceProvider.GetServices<IMarketCollector>();

                // توجه: از CollectorName استفاده کن (مطابق IMarketCollector)
                var collector = collectors.FirstOrDefault(x => string.Equals(x.CollectorName, collectorName, StringComparison.OrdinalIgnoreCase));
                if (collector == null)
                {
                    // اگر ثبت نشده، لاگ کن و ادامه بده
                    Console.WriteLine($"Collector not registered: {collectorName}");
                    continue;
                }

                var type = collector.GetType();

                foreach (var methodName in schedule.Methods ?? Enumerable.Empty<string>())
                {
                    try
                    {
                        var method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                        if (method == null)
                        {
                            Console.WriteLine($"Method '{methodName}' not found on collector '{collectorName}'");
                            continue;
                        }

                        var result = method.Invoke(collector, null);

                        // اگر متد async Task یا Task<T> بود، باید await کنیم
                        if (result is Task task)
                        {
                            await task;
                        }
                        else
                        {
                            // اگر متد sync بود، در همین‌جا اجرا شده
                        }
                    }
                    catch (TargetInvocationException tie)
                    {
                        // exception از داخل متد؛ tie.InnerException را لاگ کن
                        Console.WriteLine($"Error invoking {methodName} on {collectorName}: {tie.InnerException?.Message ?? tie.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error invoking {methodName} on {collectorName}: {ex.Message}");
                    }
                }
            }
        }

        public void Stop()
        {
            foreach (var kv in _jobs)
            {
                kv.Value.Cancel();
            }
            _jobs.Clear();
        }
    }
}