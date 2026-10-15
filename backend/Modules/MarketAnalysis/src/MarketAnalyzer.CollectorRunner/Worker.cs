namespace MarketAnalyzer.CollectorRunner
{

    public class Worker : BackgroundService
    {
        private readonly CronJobService _cron;

        public Worker(CronJobService cron)
        {
            _cron = cron;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _cron.Start();
            return Task.CompletedTask;
        }
    }
}
