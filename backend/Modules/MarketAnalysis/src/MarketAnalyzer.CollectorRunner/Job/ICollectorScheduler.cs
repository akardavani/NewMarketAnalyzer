namespace MarketAnalyzer.CollectorRunner
{
    public interface ICollectorScheduler
    {
        void RegisterJobs(CronJobRegistrar registrar);
    }
}
