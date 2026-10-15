using MarketAnalyzer.Domain;

namespace MarketAnalyzer.Collectors
{
    public abstract class BaseCollector : IMarketCollector
    {
        public abstract string CollectorName { get; }

        //protected async Task<CollectorResult> ExecuteAsync(Func<Task<CollectorResult>> action)
        //{
        //    try
        //    {
        //        return await action();
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        return new CollectorResult
        //        {
        //            Success = false,
        //            Message = "Operation was cancelled."
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new CollectorResult
        //        {
        //            Success = false,
        //            Message = ex.Message
        //        };
        //    }
        //}

        protected async Task ProcessSymbolsAsync(
            IEnumerable<string> symbols,
            Timeframe timeframe,
            MarketType marketType,
            SourceType sourceType,
            int yearsAgo,
            ICandleSourceFactory factory,
            ICandleIngestionService ingestion,
            CancellationToken cancellation,
            int batchSize = 20,
            int maxDegreeOfParallelism = 5)
        {
            if (symbols is null)
                return;

            var cleanSymbols = symbols
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Distinct()
                .ToList();

            if (cleanSymbols.Count == 0)
                return;

            var source = factory.Get(marketType, sourceType);

            foreach (var symbolBatch in cleanSymbols.Chunk(batchSize))
            {
                cancellation.ThrowIfCancellationRequested();

                using var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);

                var tasks = symbolBatch.Select(symbol =>
                    FetchSymbolCandlesSafeAsync(
                        source,
                        symbol,
                        timeframe,
                        yearsAgo,
                        semaphore,
                        cancellation));

                var batchResults = await Task.WhenAll(tasks);

                var validResults = batchResults
                    .Where(x => x is not null && x.Candles.Count > 0)
                    .Select(x => x!)
                    .ToList();

                if (validResults.Count == 0)
                    continue;

                await ingestion.SaveCandlesBatch(
                    marketType,
                    timeframe,
                    validResults,
                    cancellation);
            }
        }

        private static async Task<SymbolCandlesBatch?> FetchSymbolCandlesSafeAsync(
            ICandleSource source,
            string symbol,
            Timeframe timeframe,
            int yearsAgo,
            SemaphoreSlim semaphore,
            CancellationToken cancellation)
        {
            await semaphore.WaitAsync(cancellation);

            try
            {
                cancellation.ThrowIfCancellationRequested();

                var candles = await source.FetchCandlesAsync(
                    symbol,
                    timeframe,
                    yearsAgo,
                    cancellation);

                if (candles is null || candles.Count == 0)
                    return null;

                var symbolKey = candles.FirstOrDefault().SymbolKey;
                return new SymbolCandlesBatch(symbolKey, candles);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                // اینجا می‌تونی logger هم اضافه کنی
                return null;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
