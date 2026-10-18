using MarketAnalyzer.Abstractions;

namespace MarketIntelligence.Infrastructure
{
    /// <summary>
    /// نقطه‌ی ورود seederهای MarketIntelligence در استارتاپ (از طریق PersistenceApplicationPart).
    /// </summary>
    internal class MarketIntelligenceDataSeed : IDataSeed
    {
        private readonly IServiceProvider _services;

        public MarketIntelligenceDataSeed(IServiceProvider services)
        {
            _services = services;
        }

        public int Order => 10;

        public Task SeedAsync() => DataSeeder.SeedAllAsync(_services);
    }
}
