using System.Collections.Concurrent;
using System.Diagnostics;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <summary>
    /// اجرای کارها. Providerها فقط می‌گیرند؛ اینجا تصمیم می‌گیریم چه چیزی ذخیره شود.
    /// قانون ثابت: فقط نتیجهٔ Success جایگزین دادهٔ قبلی می‌شود و Failed/SuccessEmpty هرگز چیزی را حذف نمی‌کند.
    /// </summary>
    public sealed class MarketDataJobRunner : IMarketDataJobRunner
    {
        private readonly IServiceScopeFactory _scopes;
        private readonly IMarketJobGate _gate;
        private readonly IOptions<MarketDataOperationsOptions> _options;
        private readonly TimeProvider _time;
        private readonly ILogger<MarketDataJobRunner> _logger;

        /// <summary>سازنده.</summary>
        public MarketDataJobRunner(
            IServiceScopeFactory scopes,
            IMarketJobGate gate,
            IOptions<MarketDataOperationsOptions> options,
            TimeProvider time,
            ILogger<MarketDataJobRunner> logger)
        {
            _scopes = scopes;
            _gate = gate;
            _options = options;
            _time = time;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<MarketJobResult> RunAsync(string marketKey, MarketJobKind job, CancellationToken cancellationToken)
        {
            using var lease = _gate.TryEnter(marketKey, job);
            if (lease is null)
                return MarketJobResult.Skipped(marketKey, job, "Another run of this job is already in progress.");

            var sw = Stopwatch.StartNew();
            using var scope = _scopes.CreateScope();
            var sp = scope.ServiceProvider;

            var market = await sp.GetRequiredService<IMarketDefinitionStore>().GetByKeyAsync(marketKey, cancellationToken);
            if (market is null || !market.IsActive)
                return MarketJobResult.Skipped(marketKey, job, "Market does not exist or is inactive.");

            var result = job switch
            {
                MarketJobKind.SyncCatalog => await SyncCatalogAsync(sp, market, cancellationToken),
                MarketJobKind.IngestCandles => await IngestCandlesAsync(sp, market, cancellationToken),
                MarketJobKind.RefreshCurrentState => await RefreshCurrentStateAsync(sp, market, cancellationToken),
                MarketJobKind.RefreshOrderBook => await RefreshOrderBookAsync(sp, market, cancellationToken),
                MarketJobKind.RefreshInvestorActivity => await RefreshInvestorActivityAsync(sp, market, cancellationToken),
                _ => throw new ArgumentOutOfRangeException(nameof(job), job, null)
            };

            sw.Stop();
            return new MarketJobResult
            {
                MarketKey = result.MarketKey, Job = result.Job, Status = result.Status, Processed = result.Processed,
                Updated = result.Updated, NoData = result.NoData, FullyRefetched = result.FullyRefetched, Failed = result.Failed,
                Message = result.Message, ErrorSamples = result.ErrorSamples, Duration = sw.Elapsed
            };
        }

        // ---------------------------------------------------------------- catalog

        private async Task<MarketJobResult> SyncCatalogAsync(IServiceProvider sp, MarketDefinition market, CancellationToken ct)
        {
            var provider = await sp.GetRequiredService<IMarketProviderResolver>()
                .ResolveAsync<ISymbolCatalogProvider>(market.Key, ProviderCapability.SymbolCatalog, ct);
            if (provider is null)
                return MarketJobResult.Skipped(market.Key, MarketJobKind.SyncCatalog, "No symbol catalog provider is selected.");

            var fetched = await provider.FetchSymbolsAsync(ct);
            if (fetched.Status is FetchStatus.Failed or FetchStatus.SuccessEmpty || fetched.Data is null)
            {
                // هیچ نمادی غیرفعال نمی‌شود: «خالی» یا «خطا» دلیل حذف نیست.
                return new MarketJobResult
                {
                    MarketKey = market.Key, Job = MarketJobKind.SyncCatalog,
                    Status = fetched.Status == FetchStatus.Failed ? MarketJobStatus.Failed : MarketJobStatus.Completed,
                    NoData = fetched.Status == FetchStatus.SuccessEmpty ? 1 : 0,
                    Failed = fetched.Status == FetchStatus.Failed ? 1 : 0,
                    Message = fetched.ErrorMessage ?? "Source returned no symbols; nothing changed.",
                    ErrorSamples = fetched.ErrorMessage is null ? Array.Empty<string>() : new[] { fetched.ErrorMessage }
                };
            }

            var now = _time.GetUtcNow().UtcDateTime;
            var symbols = fetched.Data.Select(s => new MarketSymbol
            {
                MarketId = market.Id,
                SymbolKey = s.SymbolKey,
                ExternalId = s.ExternalId,
                CurrencyCode = s.CurrencyCode,
                Name = s.Name,
                PersianName = s.PersianName,
                IsActive = s.IsActive,
                MetadataJson = s.MetadataJson,
                MetadataVersion = s.MetadataVersion,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            }).ToList();

            // فهرست ناقص (Partial) فقط اضافه/به‌روز می‌کند و چیزی را غیرفعال نمی‌کند.
            var deactivateMissing = fetched.Status == FetchStatus.Success;
            var sync = await sp.GetRequiredService<IMarketSymbolStore>().SyncAsync(market.Id, symbols, deactivateMissing, ct);

            return new MarketJobResult
            {
                MarketKey = market.Key, Job = MarketJobKind.SyncCatalog, Status = MarketJobStatus.Completed,
                Processed = symbols.Count, Updated = sync.Inserted + sync.Updated,
                Message = $"inserted={sync.Inserted}, updated={sync.Updated}, deactivated={sync.Deactivated}" + (deactivateMissing ? "" : " (partial: no deactivation)")
            };
        }

        // ---------------------------------------------------------------- candles

        private async Task<MarketJobResult> IngestCandlesAsync(IServiceProvider sp, MarketDefinition market, CancellationToken ct)
        {
            var provider = await sp.GetRequiredService<IMarketProviderResolver>()
                .ResolveAsync<IHistoricalCandleProvider>(market.Key, ProviderCapability.HistoricalCandles, ct);
            if (provider is null)
                return MarketJobResult.Skipped(market.Key, MarketJobKind.IngestCandles, "No candle provider is selected.");

            var providerKey = provider.Descriptor.ProviderKey;
            var symbols = await sp.GetRequiredService<IMarketSymbolStore>().GetByMarketAsync(market.Id, onlyActive: true, ct);
            if (symbols.Count == 0)
                return MarketJobResult.Skipped(market.Key, MarketJobKind.IngestCandles, "Market has no active symbols (run SyncCatalog first).");

            var options = _options.Value;
            var pacer = new RequestPacer(provider.Descriptor.MaxRequestsPerMinute, _time);
            var results = new ConcurrentBag<SymbolIngestionResult>();
            var openCandles = new ConcurrentDictionary<string, CandleData>(StringComparer.Ordinal);
            var done = 0;
            var failures = 0;

            // اگر منبع از دسترس خارج باشد، بعد از چند شکست پشت‌سرهم اجرا را قطع می‌کنیم.
            using var abort = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var aborted = false;

            try
            {
                await Parallel.ForEachAsync(
                    symbols,
                    new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, options.MaxParallelSymbols), CancellationToken = abort.Token },
                    async (symbol, token) =>
                    {
                        await pacer.WaitAsync(token);

                        // هر نماد Scope جدا دارد: DbContext و Providerهای Scoped بین رشته‌ها مشترک نشوند.
                        using var symbolScope = _scopes.CreateScope();
                        var ssp = symbolScope.ServiceProvider;
                        var symbolProvider = ssp.GetServices<IHistoricalCandleProvider>()
                            .First(p => p.Descriptor.ProviderKey == providerKey && p.Descriptor.MarketKey == market.Key);

                        SymbolIngestionResult r;
                        try
                        {
                            r = await ssp.GetRequiredService<IHistoricalCandleIngestor>().IngestAsync(symbolProvider, symbol, token);
                        }
                        catch (Exception ex) when (ex is not OperationCanceledException)
                        {
                            // خطای یک نماد نباید بقیهٔ نمادها را متوقف کند.
                            _logger.LogError(ex, "Candle ingestion crashed for {Market}/{Symbol}", market.Key, symbol.SymbolKey);
                            r = SymbolIngestionResult.Failed(symbol.SymbolKey, ex.Message, ex);
                        }

                        results.Add(r);
                        if (r.OpenCandle is not null)
                            openCandles[symbol.SymbolKey] = r.OpenCandle;

                        var processed = Interlocked.Increment(ref done);
                        var failed = r.IsFailure ? Interlocked.Increment(ref failures) : Volatile.Read(ref failures);
                        if (processed >= options.AbortAfterConsecutiveFailures && failed == processed)
                        {
                            aborted = true;
                            abort.Cancel();
                        }
                    });
            }
            catch (OperationCanceledException) when (aborted && !ct.IsCancellationRequested)
            {
                _logger.LogError("Candle ingestion for {Market} aborted: first {Count} symbols all failed; provider '{Provider}' looks down", market.Key, done, providerKey);
            }

            // بازارهایی که Provider وضعیت لحظه‌ای ندارند، کندل امروز (ناتمام) را به‌عنوان وضعیت جاری نگه می‌دارند.
            if (!aborted && openCandles.Count > 0)
                await StoreOpenCandlesAsStateAsync(sp, market, symbols, openCandles, ct);

            var all = results.ToList();
            var errors = all.Where(r => r.IsFailure).Select(r => $"{r.SymbolKey}: {r.Message}").Take(5).ToList();
            return new MarketJobResult
            {
                MarketKey = market.Key, Job = MarketJobKind.IngestCandles,
                Status = aborted ? MarketJobStatus.Failed : MarketJobStatus.Completed,
                Processed = all.Count,
                Updated = all.Count(r => r.Outcome == IngestionOutcome.Updated),
                NoData = all.Count(r => r.Outcome == IngestionOutcome.NoData),
                FullyRefetched = all.Count(r => r.Outcome == IngestionOutcome.FullyRefetched),
                Failed = all.Count(r => r.IsFailure),
                Message = aborted ? $"Aborted: provider '{providerKey}' appears to be down." : $"provider={providerKey}, symbols={symbols.Count}, insertedCandles={all.Sum(r => r.InsertedCandles)}, deletedCandles={all.Sum(r => r.DeletedCandles)}",
                ErrorSamples = errors
            };
        }

        private async Task StoreOpenCandlesAsStateAsync(
            IServiceProvider sp, MarketDefinition market, IReadOnlyList<MarketSymbol> symbols,
            ConcurrentDictionary<string, CandleData> openCandles, CancellationToken ct)
        {
            var stateProvider = await sp.GetRequiredService<IMarketProviderResolver>()
                .ResolveAsync<ICurrentStateProvider>(market.Key, ProviderCapability.CurrentState, ct);
            if (stateProvider is not null)
                return;

            var now = _time.GetUtcNow().UtcDateTime;
            var states = new List<CurrentSymbolState>(openCandles.Count);
            foreach (var symbol in symbols)
            {
                if (!openCandles.TryGetValue(symbol.SymbolKey, out var c))
                    continue;

                states.Add(new CurrentSymbolState
                {
                    SymbolId = symbol.Id, LastPrice = c.Close, OpenPrice = c.Open, HighPrice = c.High, LowPrice = c.Low,
                    Volume = c.Volume, Value = c.Value, SourceTimestampUtc = now
                });
            }

            if (states.Count > 0)
                await sp.GetRequiredService<ICurrentSymbolStateStore>().UpsertAsync(states, ct);
        }

        // ---------------------------------------------------------------- current state / order book / investors

        private async Task<MarketJobResult> RefreshCurrentStateAsync(IServiceProvider sp, MarketDefinition market, CancellationToken ct)
        {
            const MarketJobKind job = MarketJobKind.RefreshCurrentState;
            var provider = await sp.GetRequiredService<IMarketProviderResolver>()
                .ResolveAsync<ICurrentStateProvider>(market.Key, ProviderCapability.CurrentState, ct);
            if (provider is null)
                return MarketJobResult.Skipped(market.Key, job, "No current-state provider is selected.");

            var fetched = await provider.FetchStatesAsync(ct);
            if (!IsUsable(fetched, allowPartial: true, out var early, market.Key, job))
                return early!;

            var map = await LoadSymbolMapAsync(sp, market, ct);
            var states = new List<CurrentSymbolState>(fetched.Data!.Count);
            var unknown = 0;
            foreach (var s in fetched.Data)
            {
                if (!map.TryGetValue(s.SymbolKey, out var id)) { unknown++; continue; }
                states.Add(new CurrentSymbolState
                {
                    SymbolId = id, LastPrice = s.LastPrice, ClosePrice = s.ClosePrice, OpenPrice = s.OpenPrice, HighPrice = s.HighPrice,
                    LowPrice = s.LowPrice, PreviousClosePrice = s.PreviousClosePrice, PriceLimitMin = s.PriceLimitMin,
                    PriceLimitMax = s.PriceLimitMax, Volume = s.Volume, Value = s.Value, TradeCount = s.TradeCount,
                    SourceTimestampUtc = s.SourceTimestampUtc
                });
            }

            var stats = states.Count == 0 ? default : await sp.GetRequiredService<ICurrentSymbolStateStore>().UpsertAsync(states, ct);
            return Done(market.Key, job, fetched, states.Count, stats.Inserted + stats.Existing, unknown);
        }

        private async Task<MarketJobResult> RefreshOrderBookAsync(IServiceProvider sp, MarketDefinition market, CancellationToken ct)
        {
            const MarketJobKind job = MarketJobKind.RefreshOrderBook;
            var provider = await sp.GetRequiredService<IMarketProviderResolver>()
                .ResolveAsync<IOrderBookProvider>(market.Key, ProviderCapability.OrderBook, ct);
            if (provider is null)
                return MarketJobResult.Skipped(market.Key, job, "No order-book provider is selected.");

            var fetched = await provider.FetchOrderBooksAsync(ct);
            // صف ناقص جایگزین صف کامل قبلی نمی‌شود.
            if (!IsUsable(fetched, allowPartial: false, out var early, market.Key, job))
                return early!;

            var map = await LoadSymbolMapAsync(sp, market, ct);
            var books = new Dictionary<int, IReadOnlyList<CurrentOrderBookLevel>>();
            var unknown = 0;
            foreach (var snap in fetched.Data!)
            {
                if (!map.TryGetValue(snap.SymbolKey, out var id)) { unknown++; continue; }
                books[id] = snap.Levels.Select(l => new CurrentOrderBookLevel
                {
                    SymbolId = id, Side = l.Side, LevelNumber = l.LevelNumber, Price = l.Price, Volume = l.Volume,
                    OrderCount = l.OrderCount, SnapshotTimeUtc = snap.SnapshotTimeUtc
                }).ToList();
            }

            if (books.Count > 0)
                await sp.GetRequiredService<ICurrentOrderBookStore>().ReplaceAsync(books, ct);
            return Done(market.Key, job, fetched, books.Count, books.Count, unknown);
        }

        private async Task<MarketJobResult> RefreshInvestorActivityAsync(IServiceProvider sp, MarketDefinition market, CancellationToken ct)
        {
            const MarketJobKind job = MarketJobKind.RefreshInvestorActivity;
            var provider = await sp.GetRequiredService<IMarketProviderResolver>()
                .ResolveAsync<IInvestorActivityProvider>(market.Key, ProviderCapability.InvestorActivity, ct);
            if (provider is null)
                return MarketJobResult.Skipped(market.Key, job, "No investor-activity provider is selected.");

            var fetched = await provider.FetchInvestorActivitiesAsync(ct);
            if (!IsUsable(fetched, allowPartial: true, out var early, market.Key, job))
                return early!;

            var map = await LoadSymbolMapAsync(sp, market, ct);
            var rows = new List<InvestorTradingActivity>(fetched.Data!.Count);
            var unknown = 0;
            foreach (var a in fetched.Data)
            {
                if (!map.TryGetValue(a.SymbolKey, out var id)) { unknown++; continue; }
                rows.Add(new InvestorTradingActivity
                {
                    SymbolId = id, TradingDate = a.TradingDate,
                    IndividualBuyVolume = a.IndividualBuyVolume, IndividualBuyValue = a.IndividualBuyValue, IndividualBuyCount = a.IndividualBuyCount,
                    IndividualSellVolume = a.IndividualSellVolume, IndividualSellValue = a.IndividualSellValue, IndividualSellCount = a.IndividualSellCount,
                    InstitutionalBuyVolume = a.InstitutionalBuyVolume, InstitutionalBuyValue = a.InstitutionalBuyValue, InstitutionalBuyCount = a.InstitutionalBuyCount,
                    InstitutionalSellVolume = a.InstitutionalSellVolume, InstitutionalSellValue = a.InstitutionalSellValue, InstitutionalSellCount = a.InstitutionalSellCount
                });
            }

            var stats = rows.Count == 0 ? default : await sp.GetRequiredService<IInvestorActivityStore>().UpsertAsync(rows, ct);
            return Done(market.Key, job, fetched, rows.Count, stats.Inserted + stats.Existing, unknown);
        }

        // ---------------------------------------------------------------- helpers

        private static async Task<Dictionary<string, int>> LoadSymbolMapAsync(IServiceProvider sp, MarketDefinition market, CancellationToken ct)
        {
            var symbols = await sp.GetRequiredService<IMarketSymbolStore>().GetByMarketAsync(market.Id, onlyActive: false, ct);
            return symbols.ToDictionary(s => s.SymbolKey, s => s.Id, StringComparer.Ordinal);
        }

        /// <summary>آیا نتیجه قابل‌ذخیره است؟ در غیر این صورت نتیجهٔ زودهنگام (شکست یا بدون‌داده) را می‌سازد.</summary>
        private static bool IsUsable<T>(FetchResult<T> fetched, bool allowPartial, out MarketJobResult? early, string marketKey, MarketJobKind job)
        {
            early = null;
            if (fetched.Status == FetchStatus.Success && fetched.Data is not null)
                return true;
            if (fetched.Status == FetchStatus.Partial && allowPartial && fetched.Data is not null)
                return true;

            var failed = fetched.Status is FetchStatus.Failed or FetchStatus.Partial;
            early = new MarketJobResult
            {
                MarketKey = marketKey, Job = job,
                Status = failed ? MarketJobStatus.Failed : MarketJobStatus.Completed,
                NoData = failed ? 0 : 1, Failed = failed ? 1 : 0,
                Message = fetched.ErrorMessage ?? "Source returned no data; nothing changed.",
                ErrorSamples = fetched.ErrorMessage is null ? Array.Empty<string>() : new[] { fetched.ErrorMessage }
            };
            return false;
        }

        private static MarketJobResult Done<T>(string marketKey, MarketJobKind job, FetchResult<T> fetched, int processed, int updated, int unknown)
            => new()
            {
                MarketKey = marketKey, Job = job, Status = MarketJobStatus.Completed, Processed = processed, Updated = updated,
                Message = (unknown > 0 ? $"{unknown} source symbols are not in the catalog and were skipped. " : "")
                          + (fetched.Status == FetchStatus.Partial ? $"Partial: {fetched.ErrorMessage}" : "")
            };
    }
}
