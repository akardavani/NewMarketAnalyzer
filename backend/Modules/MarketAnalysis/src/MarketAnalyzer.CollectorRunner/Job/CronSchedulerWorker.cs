namespace MarketAnalyzer.CollectorRunner
{
    public class CronSchedulerWorker : BackgroundService
    {
        private readonly CronJobRegistrar _registrar;
        private readonly ICollectorScheduler _scheduler;

        public CronSchedulerWorker(CronJobRegistrar registrar, ICollectorScheduler scheduler)
        {
            _registrar = registrar;
            _scheduler = scheduler;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Jobها را ثبت می‌کنیم
            _scheduler.RegisterJobs(_registrar);
            return ExecuteLoopAsync(stoppingToken);
        }

        private async Task ExecuteLoopAsync(CancellationToken stoppingToken)
        {
            // مقداردهی اولیه NextRun برای هر Job
            var jobs = _registrar.GetAll()
                .Select(j => new ScheduledJob
                {
                    Registration = j,
                    NextRun = j.Cron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Utc)
                })
                .ToList();

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;

                foreach (var job in jobs)
                {
                    if (job.NextRun.HasValue && job.NextRun.Value <= now)
                    {
                        // اجرا کردن Job
                        _ = Task.Run(job.Registration.Job, stoppingToken);

                        // محاسبه زمان بعدی اجرای Job
                        job.NextRun = job.Registration.Cron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Utc);
                    }
                }

                await Task.Delay(500, stoppingToken); // چک کردن هر نیم ثانیه
            }
        }

        // کلاس داخلی برای نگه داشتن زمان بعدی اجرای هر Job
        private class ScheduledJob
        {
            public CronJobRegistration Registration { get; set; }
            public DateTime? NextRun { get; set; }
        }
    }
}
