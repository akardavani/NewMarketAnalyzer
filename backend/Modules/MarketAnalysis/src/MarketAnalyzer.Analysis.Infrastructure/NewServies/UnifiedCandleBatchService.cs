using MarketAnalyzer.Domain;

namespace MarketAnalyzer.Analysis.Infrastructure
{
    public class UnifiedCandleBatchService : IUnifiedCandleBatchService
    {
        private readonly IUnifiedCandleRepository _repository;

        public UnifiedCandleBatchService(IUnifiedCandleRepository repository)
        {
            _repository = repository;
        }

        public async Task SaveCandlesAsync(
            IEnumerable<UnifiedCandleRecord> candles,
            CancellationToken cancellationToken)
        {
            var list = candles?
                .Where(x => x != null)
                .GroupBy(x => new { x.MarketId, x.SymbolKey, x.TimeframeId, x.Date })
                .Select(g => g.Last())
                .ToList();

            if (list == null || list.Count == 0)
                return;

            await _repository.UpsertManyAsync(list, cancellationToken);
        }

        public async Task SaveMappedCandlesAsync<TSource>(
            IEnumerable<TSource> source,
            Func<TSource, bool> predicate,
            Func<TSource, UnifiedCandleRecord> mapper,
            CancellationToken cancellationToken)
        {
            if (source == null)
                return;

            var candles = source
                .Where(predicate)
                .Select(mapper)
                .Where(x => x != null)
                .GroupBy(x => new { x.MarketId, x.SymbolKey, x.TimeframeId, x.Date })
                .Select(g => g.Last())
                .ToList();

            if (candles.Count == 0)
                return;

            await _repository.UpsertManyAsync(candles, cancellationToken);
        }
    }
}
