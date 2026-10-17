using MarketAnalyzer.Collectors;

namespace MarketAnalyzer.CollectorRunner
{
    public class CollectorJob
    {
        private readonly IServiceProvider _provider;
        private readonly CollectorConfig _collector;
        private readonly TimeframeSchedule _timeframe;

        public CollectorJob(IServiceProvider provider, CollectorConfig collector, TimeframeSchedule timeframe)
        {
            _provider = provider;
            _collector = collector;
            _timeframe = timeframe;
        }

        public async Task ExecuteAsync()
        {
            // پیدا کردن type واقعی collector
            var type = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .First(t => t.Name == _collector.Name);

            // resolve instance از scoped provider
            var collector = (IMarketCollector)_provider.GetRequiredService(type);

            foreach (var methodName in _timeframe.Methods)
            {
                var method = type.GetMethod(methodName);

                if (method != null)
                {
                    var task = (Task)method.Invoke(collector, null);
                    await task;
                }
            }
        }
    }
}
