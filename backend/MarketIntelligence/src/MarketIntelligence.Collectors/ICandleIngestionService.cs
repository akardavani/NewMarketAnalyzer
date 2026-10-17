using MarketIntelligence.Domain;

namespace MarketIntelligence.Collectors
{
    public interface ICandleIngestionService
    {
        Task SaveCandles(
            MarketType market,
            string symbol,
            Timeframe timeframe,
            List<UnifiedCandleRecord> candles,
            CancellationToken ct);

        Task SaveCandlesBatch(
        MarketType market,
        Timeframe timeframe,
        IReadOnlyCollection<SymbolCandlesBatch> batches,
        CancellationToken ct);

        Task BulkDeleteAsync(
            MarketType marketType,
            Timeframe timeframe,
            CancellationToken ct);
    }


    public sealed record SymbolCandlesBatch(string Symbol,List<UnifiedCandleRecord> Candles);

}
