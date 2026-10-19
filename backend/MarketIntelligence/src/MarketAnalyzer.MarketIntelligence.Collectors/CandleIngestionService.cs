using MarketAnalyzer.MarketIntelligence.Domain;
using MarketAnalyzer.MarketIntelligence.Infrastructure;

namespace MarketAnalyzer.MarketIntelligence.Collectors
{
    public class CandleIngestionService : ICandleIngestionService
    {
        private readonly IUnifiedCandleRepository _repo;

        public CandleIngestionService(IUnifiedCandleRepository repo)
        {
            _repo = repo;
        }

        public async Task BulkDeleteAsync(
            MarketType marketType,
            Timeframe timeframe,
            CancellationToken ct)
        {
            await _repo.BulkDeleteAsync(marketType, timeframe, ct);
        }

        public async Task SaveCandles(
            MarketType market,
            string symbol,
            Timeframe timeframe,
            List<UnifiedCandleRecord> candles,
            CancellationToken ct)
        {
            if (candles.Count == 0)
                return;

            foreach (var candle in candles)
            {
                candle.MarketId = market;
                candle.SymbolKey = symbol;
                candle.TimeframeId = timeframe;
            }

            await _repo.UpsertManyAsync(candles, ct);
        }

        public async Task SaveCandlesBatch(
            MarketType market,
            Timeframe timeframe,
            IReadOnlyCollection<SymbolCandlesBatch> batches,
            CancellationToken ct)
        {
            if (batches.Count == 0)
                return;

            var allCandles = new List<UnifiedCandleRecord>();

            foreach (var batch in batches)
            {
                if (string.IsNullOrWhiteSpace(batch.Symbol))
                    continue;

                if (batch.Candles.Count == 0)
                    continue;

                foreach (var candle in batch.Candles)
                {
                    candle.MarketId = market;
                    candle.SymbolKey = batch.Symbol;
                    candle.TimeframeId = timeframe;
                }

                allCandles.AddRange(batch.Candles);
            }

            if (allCandles.Count == 0)
                return;

            await _repo.UpsertManyAsync(allCandles, ct);
        }
    }
}
