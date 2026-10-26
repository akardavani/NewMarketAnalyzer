using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>
    /// پیاده‌سازی <see cref="IHistoricalCandleIngestor"/>.
    /// <para>جریان: ۱) اگر چیزی ذخیره نشده، دریافت کامل. ۲) وگرنه دریافت پنجرهٔ هم‌پوشانی، اعتبارسنجی، جداکردن کندل ناتمام،
    /// تشخیص drift (فقط منبع تعدیل‌شده). ۳) drift ثابت → دریافت کامل و جایگزینی اتمی همان نماد (روزانه + هفتگی + ماهانه در یک تراکنش)
    /// و ثبت رویداد شرکتی. ۴) وگرنه Upsert و بازسازی هفتگی/ماهانه از ابتدای بازهٔ تغییرکرده.</para>
    /// <para>قاعدهٔ ایمنی: فقط دریافت کاملاً موفق (Success) می‌تواند چیزی را جایگزین کند؛ شکست، خالی و ناقص هرگز حذف نمی‌کنند.</para>
    /// </summary>
    public sealed class HistoricalCandleIngestor : IHistoricalCandleIngestor
    {
        private readonly IHistoricalCandleStore _candles;
        private readonly ICorporateActionStore _actions;
        private readonly ICandleValidator _validator;
        private readonly IAdjustmentDriftDetector _driftDetector;
        private readonly ICandleAggregator _aggregator;
        private readonly ITradingCalendarFactory _calendars;
        private readonly IngestionOptions _options;
        private readonly TimeProvider _time;
        private readonly ILogger<HistoricalCandleIngestor> _logger;

        /// <summary>سازنده.</summary>
        public HistoricalCandleIngestor(
            IHistoricalCandleStore candles,
            ICorporateActionStore actions,
            ICandleValidator validator,
            IAdjustmentDriftDetector driftDetector,
            ICandleAggregator aggregator,
            ITradingCalendarFactory calendars,
            IOptions<IngestionOptions> options,
            TimeProvider time,
            ILogger<HistoricalCandleIngestor> logger)
        {
            _candles = candles;
            _actions = actions;
            _validator = validator;
            _driftDetector = driftDetector;
            _aggregator = aggregator;
            _calendars = calendars;
            _options = options.Value;
            _time = time;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<SymbolIngestionResult> IngestAsync(IHistoricalCandleProvider provider, MarketSymbol symbol, CancellationToken cancellationToken)
        {
            try
            {
                return await IngestCoreAsync(provider, symbol, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // شامل حالت «برنامه قفل است» (FundDbContext) هم می‌شود؛ job بعداً دوباره تلاش می‌کند.
                _logger.LogError(ex, "Candle ingestion failed for {Provider}/{Symbol}", provider.Descriptor.ProviderKey, symbol.SymbolKey);
                return SymbolIngestionResult.Failed(symbol.SymbolKey, ex.Message, ex);
            }
        }

        private async Task<SymbolIngestionResult> IngestCoreAsync(IHistoricalCandleProvider provider, MarketSymbol symbol, CancellationToken ct)
        {
            var descriptor = provider.Descriptor;
            var calendar = _calendars.Create(descriptor.CalendarKind, descriptor.TimeZoneId);
            var baseTimeframe = ResolveBaseTimeframe(descriptor);
            var derived = GetDerivedTimeframes(baseTimeframe);
            var now = _time.GetUtcNow().UtcDateTime;

            var stored = await _candles.GetLastAsync(symbol.Id, baseTimeframe, _options.OverlapWindowCandles, ct);
            if (stored.Count == 0)
                return await RefreshFullAsync(provider, symbol, calendar, baseTimeframe, derived, now, storedCount: 0, drift: null, ct);

            var fetch = await provider.FetchCandlesAsync(new CandleRequest
            {
                SymbolKey = symbol.SymbolKey,
                ExternalId = symbol.ExternalId,
                Timeframe = baseTimeframe,
                LastCandlesCount = _options.OverlapWindowCandles
            }, ct);

            if (fetch.Status == FetchStatus.Failed)
                return SymbolIngestionResult.Failed(symbol.SymbolKey, fetch.ErrorMessage ?? "Fetch failed.", fetch.Exception);

            if (fetch.Status == FetchStatus.SuccessEmpty || fetch.Data == null)
                return new SymbolIngestionResult { SymbolKey = symbol.SymbolKey, Outcome = IngestionOutcome.NoData, Message = "Source returned no candles." };

            var validated = _validator.Validate(fetch.Data, calendar, now);
            LogIssues(symbol, validated);

            if (validated.Closed.Count == 0)
            {
                return new SymbolIngestionResult
                {
                    SymbolKey = symbol.SymbolKey,
                    Outcome = IngestionOutcome.NoData,
                    OpenCandle = validated.Open,
                    Message = "No closed candles in response."
                };
            }

            // تشخیص drift فقط وقتی معنا دارد که منبع سری تعدیل‌شده می‌دهد و پاسخ کامل (نه ناقص) است.
            if (descriptor.Adjustment == AdjustmentMode.AlreadyAdjusted && fetch.Status == FetchStatus.Success)
            {
                var drift = _driftDetector.Detect(stored, validated.Closed);
                if (drift.Kind == DriftKind.ConstantRatio)
                {
                    _logger.LogInformation("Adjustment drift on {Symbol}: ratio {Ratio}; refetching full history", symbol.SymbolKey, drift.Ratio);
                    return await RefreshFullAsync(provider, symbol, calendar, baseTimeframe, derived, now, stored.Count, drift, ct);
                }

                if (drift.Kind == DriftKind.Inconsistent)
                {
                    _logger.LogWarning("Inconsistent candle revisions on {Symbol}: {Mismatched}/{Compared} differ; upserting as-is",
                        symbol.SymbolKey, drift.MismatchedCount, drift.ComparedCount);
                }
            }

            var stats = await _candles.UpsertAsync(symbol.Id, baseTimeframe, validated.Closed.Select(c => ToEntity(symbol.Id, baseTimeframe, c)).ToList(), ct);

            // کندلی که قبلاً ذخیره شده ولی منبع در همان بازه دیگر نمی‌دهد، حذف می‌شود (فقط با پاسخ کامل Success).
            var deleted = fetch.Status == FetchStatus.Success
                ? await DeleteVanishedAsync(symbol, baseTimeframe, stored, validated.Closed, ct)
                : 0;

            await RebuildDerivedAsync(symbol, calendar, baseTimeframe, derived, validated.Closed[0].OpenTimeUtc, now, deletedAny: deleted > 0, ct);

            return new SymbolIngestionResult
            {
                SymbolKey = symbol.SymbolKey,
                Outcome = IngestionOutcome.Updated,
                InsertedCandles = stats.Inserted,
                DeletedCandles = deleted,
                OpenCandle = validated.Open
            };
        }

        /// <summary>
        /// کندل‌های ذخیره‌شده‌ای را که داخل بازهٔ پاسخ تازه قرار دارند ولی در آن نیستند حذف می‌کند.
        /// بازهٔ مقایسه از اولین تا آخرین کندل بستهٔ پاسخ است؛ کندل‌های قدیمی‌تر از پنجره دست نمی‌خورند.
        /// اگر بیش از <see cref="IngestionOptions.MaxDeletedFraction"/> از پنجره حذف‌شدنی باشد، حذف نمی‌کنیم
        /// (احتمالاً پاسخ منبع ناقص است، نه اینکه واقعاً این‌همه کندل پاک شده باشد).
        /// </summary>
        private async Task<int> DeleteVanishedAsync(MarketSymbol symbol, Timeframe timeframe, IReadOnlyList<HistoricalCandle> stored, IReadOnlyList<CandleData> incomingClosed, CancellationToken ct)
        {
            var first = incomingClosed[0].OpenTimeUtc;
            var last = incomingClosed[^1].OpenTimeUtc;
            var incomingTimes = incomingClosed.Select(c => c.OpenTimeUtc).ToHashSet();

            var vanished = stored
                .Where(s => s.OpenTimeUtc >= first && s.OpenTimeUtc <= last && !incomingTimes.Contains(s.OpenTimeUtc))
                .Select(s => s.OpenTimeUtc)
                .ToList();

            if (vanished.Count == 0)
                return 0;

            if (vanished.Count > _options.MaxDeletedFraction * Math.Max(1, stored.Count))
            {
                _logger.LogWarning("Skipping deletion of {Count} vanished candles for {Symbol}: exceeds allowed fraction of the window",
                    vanished.Count, symbol.SymbolKey);
                return 0;
            }

            _logger.LogInformation("Deleting {Count} candles for {Symbol} that the source no longer returns", vanished.Count, symbol.SymbolKey);
            return await _candles.DeleteByOpenTimesAsync(symbol.Id, timeframe, vanished, ct);
        }

        /// <summary>
        /// دریافت کامل و جایگزینی اتمی سری‌های یک نماد. اگر دریافت موفق و سالم نباشد، سری ذخیره‌شده دست‌نخورده می‌ماند.
        /// </summary>
        private async Task<SymbolIngestionResult> RefreshFullAsync(
            IHistoricalCandleProvider provider, MarketSymbol symbol, ITradingCalendar calendar,
            Timeframe baseTimeframe, IReadOnlyList<Timeframe> derived, DateTime now,
            int storedCount, DriftAssessment? drift, CancellationToken ct)
        {
            var fetch = await provider.FetchCandlesAsync(new CandleRequest
            {
                SymbolKey = symbol.SymbolKey,
                ExternalId = symbol.ExternalId,
                Timeframe = baseTimeframe,
                YearsBack = _options.FullHistoryYears
            }, ct);

            // فقط Success کامل اجازهٔ جایگزینی دارد. Partial/Empty/Failed → هیچ حذفی انجام نمی‌شود.
            if (fetch.Status != FetchStatus.Success || fetch.Data == null)
            {
                return SymbolIngestionResult.Failed(symbol.SymbolKey,
                    $"Full history fetch was {fetch.Status}; stored series left untouched. {fetch.ErrorMessage}", fetch.Exception);
            }

            var validated = _validator.Validate(fetch.Data, calendar, now);
            LogIssues(symbol, validated);

            var rejectedFraction = validated.RejectedCount / (double)Math.Max(1, fetch.Count);
            if (rejectedFraction > _options.MaxRejectedFraction)
            {
                return SymbolIngestionResult.Failed(symbol.SymbolKey,
                    $"Too many invalid candles in full history ({validated.RejectedCount}/{fetch.Count}); replacement skipped.");
            }

            // محافظ در برابر پاسخ «موفق ولی کوتاه‌شده»: سری جدید نباید از پنجرهٔ فعلی ذخیره‌شده کوتاه‌تر باشد.
            if (validated.Closed.Count == 0 || validated.Closed.Count < storedCount)
            {
                return SymbolIngestionResult.Failed(symbol.SymbolKey,
                    $"Full history has {validated.Closed.Count} closed candles, fewer than the {storedCount} already stored; replacement skipped.");
            }

            var baseRows = validated.Closed.Select(c => ToEntity(symbol.Id, baseTimeframe, c)).ToList();
            var series = new Dictionary<Timeframe, IReadOnlyCollection<HistoricalCandle>> { [baseTimeframe] = baseRows };

            foreach (var timeframe in derived)
            {
                var rows = _aggregator.Aggregate(baseRows, timeframe, calendar, now);
                if (rows.Count > 0)
                    series[timeframe] = rows;
            }

            // روزانه + هفتگی + ماهانه در یک تراکنش: یا همه جایگزین می‌شوند یا هیچ‌کدام.
            await _candles.ReplaceSeriesAsync(symbol.Id, series, ct);

            if (drift is { Kind: DriftKind.ConstantRatio })
                await RecordDriftAsync(symbol, calendar, drift, ct);

            return new SymbolIngestionResult
            {
                SymbolKey = symbol.SymbolKey,
                Outcome = IngestionOutcome.FullyRefetched,
                InsertedCandles = baseRows.Count,
                DriftRatio = drift is { Kind: DriftKind.ConstantRatio } ? drift.Ratio : null,
                OpenCandle = validated.Open
            };
        }

        /// <summary>هفتگی/ماهانه را از ابتدای قدیمی‌ترین بازهٔ تغییرکرده دوباره می‌سازد (idempotent؛ اجرای تکراری بی‌ضرر است).</summary>
        private async Task RebuildDerivedAsync(
            MarketSymbol symbol, ITradingCalendar calendar, Timeframe baseTimeframe,
            IReadOnlyList<Timeframe> derived, DateTime earliestChangedUtc, DateTime now, bool deletedAny, CancellationToken ct)
        {
            if (derived.Count == 0)
                return;

            var from = derived.Select(tf => calendar.AlignToPeriodStart(earliestChangedUtc, tf)).Min();
            var baseRows = await _candles.GetFromAsync(symbol.Id, baseTimeframe, from, ct);

            foreach (var timeframe in derived)
            {
                // اگر کندل پایه‌ای حذف شده، بازه‌های مشتقِ بعد از نقطهٔ تغییر باید از صفر ساخته شوند، وگرنه بازه‌ای که
                // دیگر هیچ کندل پایه‌ای ندارد در جدول می‌ماند. (غیراتمی است ولی با اجرای بعدی خودترمیم می‌شود.)
                if (deletedAny)
                    await _candles.DeleteFromAsync(symbol.Id, timeframe, calendar.AlignToPeriodStart(earliestChangedUtc, timeframe), ct);

                var rows = _aggregator.Aggregate(baseRows, timeframe, calendar, now);
                if (rows.Count > 0)
                    await _candles.UpsertAsync(symbol.Id, timeframe, rows, ct);
            }
        }

        /// <summary>
        /// رویداد تشخیص‌داده‌شده از drift را ثبت می‌کند (نوع دقیقش از روی قیمت معلوم نیست → Other). شکست ثبت،
        /// نتیجهٔ ingestion را خراب نمی‌کند چون سری تعدیل‌شده قبلاً درست ذخیره شده است.
        /// </summary>
        private async Task RecordDriftAsync(MarketSymbol symbol, ITradingCalendar calendar, DriftAssessment drift, CancellationToken ct)
        {
            try
            {
                var effectiveUtc = drift.EffectiveOpenTimeUtc ?? _time.GetUtcNow().UtcDateTime;
                await _actions.UpsertAsync(symbol.Id, new[]
                {
                    new CorporateAction
                    {
                        SymbolId = symbol.Id,
                        ActionType = CorporateActionType.Other,
                        EffectiveDate = calendar.GetLocalDate(effectiveUtc),
                        IsDateApproximate = drift.IsEffectiveTimeApproximate,
                        AdjustmentFactor = drift.Ratio,
                        Origin = CorporateActionOrigin.DetectedFromDrift,
                        Description = $"Detected from adjustment drift: ratio {drift.Ratio:0.######} over {drift.ComparedCount} compared candles."
                    }
                }, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Failed to record drift corporate action for {Symbol}", symbol.SymbolKey);
            }
        }

        /// <summary>
        /// تایم‌فریم پایه: اگر منبع روزانه دارد همان؛ وگرنه ریزترین تایم‌فریم بومی (مثلاً ماهانهٔ مسکن).
        /// مقدار enum بزرگ‌تر یعنی ریزتر.
        /// </summary>
        private static Timeframe ResolveBaseTimeframe(ProviderDescriptor descriptor)
        {
            if (descriptor.NativeTimeframes.Contains(Timeframe.Daily))
                return Timeframe.Daily;

            if (descriptor.NativeTimeframes.Count == 0)
                throw new InvalidOperationException($"Provider {descriptor.ProviderKey} declares no native timeframes.");

            return descriptor.NativeTimeframes.Max();
        }

        /// <summary>هفتگی و ماهانه فقط از روی روزانه ساخته می‌شوند؛ پایهٔ غیرروزانه مشتق ندارد.</summary>
        private static IReadOnlyList<Timeframe> GetDerivedTimeframes(Timeframe baseTimeframe)
            => baseTimeframe == Timeframe.Daily ? new[] { Timeframe.Weekly, Timeframe.Mountly } : Array.Empty<Timeframe>();

        private static HistoricalCandle ToEntity(int symbolId, Timeframe timeframe, CandleData c) => new()
        {
            SymbolId = symbolId,
            Timeframe = timeframe,
            OpenTimeUtc = c.OpenTimeUtc,
            Open = c.Open,
            High = c.High,
            Low = c.Low,
            Close = c.Close,
            Volume = c.Volume,
            Value = c.Value
        };

        private void LogIssues(MarketSymbol symbol, ValidatedCandles validated)
        {
            if (validated.RejectedCount > 0 || validated.RepairedCount > 0)
            {
                _logger.LogWarning("Candle validation for {Symbol}: rejected {Rejected}, repaired {Repaired}. Samples: {Issues}",
                    symbol.SymbolKey, validated.RejectedCount, validated.RepairedCount, string.Join("; ", validated.Issues));
            }
        }
    }
}
