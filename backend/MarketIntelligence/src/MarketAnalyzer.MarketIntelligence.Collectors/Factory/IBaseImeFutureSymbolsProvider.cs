using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.Collector
{
    public interface IBaseImeFutureSymbolsProvider
    {
        Task<List<CandleModel>> GetBaseImeFutureCandlesAsync(string symbol);
        Task<ImeSymbol> GetSymbolsAsync(string symbol, Timeframe timeframe, int yearsAgo, CancellationToken cancellation);
    }
}
