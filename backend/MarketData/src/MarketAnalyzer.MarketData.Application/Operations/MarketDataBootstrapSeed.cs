using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.MarketData.Application.Operations
{
    /// <summary>بعد از seedهای پایه، Providerها و برنامهٔ کارهای پیش‌فرض را در MarketSettings ثبت می‌کند.</summary>
    public sealed class MarketDataBootstrapSeed : IDataSeed
    {
        private readonly IMarketBootstrapService _bootstrap;

        public MarketDataBootstrapSeed(IMarketBootstrapService bootstrap) => _bootstrap = bootstrap;

        public int Order => 20;

        public Task SeedAsync() => _bootstrap.BootstrapAsync(CancellationToken.None);
    }
}
