namespace MarketAnalyzer.CollectorRunner
{
    public class CollectorScheduler : ICollectorScheduler
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly CollectorScheduleSettings _settings;

        public CollectorScheduler(IServiceScopeFactory scopeFactory, CollectorScheduleSettings settings)
        {
            _scopeFactory = scopeFactory;
            _settings = settings;
        }

        public void RegisterJobs(CronJobRegistrar registrar)
        {
            foreach (var collectorConfig in _settings.Collectors.Where(c => c.Enabled))
            {
                foreach (var tf in collectorConfig.Timeframes)
                {
                    var id = $"{collectorConfig.Name}-{tf.Timeframe}";

                    registrar.Register(
                        id,
                        tf.Cron,
                        async () =>
                        {
                            using var scope = _scopeFactory.CreateScope();
                            var provider = scope.ServiceProvider;

                            Console.WriteLine($"[{DateTime.Now}] Starting Collector {collectorConfig.Name} for {tf.Timeframe}");

                            var job = new CollectorJob(provider, collectorConfig, tf);
                            await job.ExecuteAsync();

                            Console.WriteLine($"[{DateTime.Now}] Finished Collector {collectorConfig.Name} for {tf.Timeframe}");
                        });
                }
            }
        }
    }

}
