using Cronos;
namespace MarketAnalyzer.CollectorRunner
{  
    public class CronJobRegistrar
    {
        private readonly List<CronJobRegistration> _registrations = new();

        public void Register(string id, string cronExpression, Func<Task> job)
        {
            //var cron = CronExpression.Parse(cronExpression);
            var cron = CronExpression.Parse(cronExpression, CronFormat.IncludeSeconds);

            _registrations.Add(new CronJobRegistration
            {
                Id = id,
                Cron = cron,
                Job = job
            });
        }

        public IEnumerable<CronJobRegistration> GetAll()
        {
            return _registrations;
        }
    }
}
