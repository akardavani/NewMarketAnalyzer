using Cronos;

namespace MarketAnalyzer.CollectorRunner
{
    public class CronJobRegistration
    {
        public string Id { get; set; }
        public CronExpression Cron { get; set; }
        public Func<Task> Job { get; set; }
    }

}
