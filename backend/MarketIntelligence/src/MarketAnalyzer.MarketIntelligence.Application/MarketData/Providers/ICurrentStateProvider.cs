using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>قابلیت دریافت آخرین قیمت و وضعیت همهٔ نمادهای بازار.</summary>
    public interface ICurrentStateProvider : IMarketDataProvider
    {
        /// <summary>آخرین وضعیت نمادهای بازار را برمی‌گرداند.</summary>
        Task<FetchResult<IReadOnlyList<SymbolStateInfo>>> FetchStatesAsync(CancellationToken cancellationToken);
    }
}
