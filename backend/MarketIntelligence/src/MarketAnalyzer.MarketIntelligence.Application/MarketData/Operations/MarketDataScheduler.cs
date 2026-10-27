using System.Collections.Concurrent;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <summary>
    /// زمان‌بند عمومی: به‌جای یک کلاس برای هر کار، برای هر (بازار × نوع کار) از روی MarketSettings تصمیم می‌گیرد.
    /// <list type="bullet">
    /// <item>اجرای هم‌زمان یک کار برای یک بازار نداریم (وضعیت درون‌حافظه + <see cref="IMarketJobGate"/>).</item>
    /// <item>شکست: تلاش بعدی با backoff نمایی (فاصله × ۲^تعداد شکست، حداکثر یک ساعت)؛ موفقیت شمارنده را صفر می‌کند.</item>
    /// <item>قفل برنامه (<see cref="ClientExposedException"/> از SaveChanges): شکست حساب نمی‌شود؛ ۳۰ ثانیه بعد دوباره امتحان می‌شود.</item>
    /// <item>اگر Provider انتخاب نشده باشد، کار «Skipped» می‌شود و ۵ دقیقه بعد دوباره بررسی می‌شود (خطا نیست).</item>
    /// <item>کارهای غیر از SyncCatalog تا وقتی فهرست نماد بازار آماده نشده منتظر می‌مانند.</item>
    /// <item>کلید <c>DisableTasks=true</c> در پیکربندی همه را متوقف می‌کند (همان کلید زمان‌بند قدیمی).</item>
    /// </list>
    /// </summary>
    public sealed class MarketDataScheduler : BackgroundService
    {
        private static readonly TimeSpan MaxBackoff = TimeSpan.FromHours(1);
        private static readonly TimeSpan SkippedRetry = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan LockedRetry = TimeSpan.FromSeconds(30);

        private readonly IServiceScopeFactory _scopes;
        private readonly IMarketDataJobRunner _runner;
        private readonly IConfiguration _configuration;
        private readonly IOptions<MarketDataOperationsOptions> _options;
        private readonly TimeProvider _time;
        private readonly ILogger<MarketDataScheduler> _logger;

        private readonly Dictionary<(string Market, MarketJobKind Job), JobState> _states = new();
        private readonly ConcurrentDictionary<(string, MarketJobKind), Task> _running = new();
        private readonly HashSet<string> _catalogReady = new(StringComparer.Ordinal);
        private readonly Dictionary<string, DateTimeOffset> _catalogCheckedAt = new(StringComparer.Ordinal);
        private SemaphoreSlim _concurrency = null!;

        private List<MarketSnapshot> _snapshot = new();
        private DateTimeOffset _snapshotExpires = DateTimeOffset.MinValue;

        /// <summary>سازنده.</summary>
        public MarketDataScheduler(
            IServiceScopeFactory scopes,
            IMarketDataJobRunner runner,
            IConfiguration configuration,
            IOptions<MarketDataOperationsOptions> options,
            TimeProvider time,
            ILogger<MarketDataScheduler> logger)
        {
            _scopes = scopes;
            _runner = runner;
            _configuration = configuration;
            _options = options;
            _time = time;
            _logger = logger;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // اجرای همگام قبل از اولین await نباید راه‌اندازی برنامه را مسدود کند.
            await Task.Yield();

            _concurrency = new SemaphoreSlim(Math.Max(1, _options.Value.MaxConcurrentJobs));

            await BootstrapWithRetryAsync(stoppingToken);

            using var timer = new PeriodicTimer(_options.Value.SchedulerTick);
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    if (_configuration.GetValue("DisableTasks", false))
                        continue;

                    try
                    {
                        await TickAsync(stoppingToken);
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        // یک دور خراب (مثلاً قطعی لحظه‌ای دیتابیس) نباید زمان‌بند را بکُشد.
                        _logger.LogError(ex, "Scheduler tick failed");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // خاموش‌شدن عادی
            }

            // صبر برای پایان کارهای در حال اجرا (توکن لغو به آن‌ها هم رسیده است).
            await Task.WhenAll(_running.Values.Select(t => t.ContinueWith(_ => { }, TaskScheduler.Default)));
        }

        private async Task BootstrapWithRetryAsync(CancellationToken ct)
        {
            var delay = TimeSpan.FromSeconds(5);
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopes.CreateScope();
                    await scope.ServiceProvider.GetRequiredService<IMarketBootstrapService>().BootstrapAsync(ct);
                    return;
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    // معمولاً دیتابیس هنوز بالا نیامده یا مایگریشن اجرا نشده است.
                    _logger.LogError(ex, "Market bootstrap failed; retrying in {Delay}", delay);
                    try { await Task.Delay(delay, ct); } catch (OperationCanceledException) { return; }
                    delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 2, 120));
                }
            }
        }

        private async Task TickAsync(CancellationToken ct)
        {
            var now = _time.GetUtcNow();
            if (now >= _snapshotExpires)
            {
                _snapshot = await LoadSnapshotAsync(ct);
                _snapshotExpires = now + _options.Value.SettingsCacheDuration;
            }

            foreach (var market in _snapshot)
            {
                foreach (var job in Enum.GetValues<MarketJobKind>())
                {
                    if (!market.Capabilities.Contains(ProviderCapabilities.For(job)))
                        continue;

                    var plan = MarketJobPlan.Resolve(job, market.Calendar, market.Settings);
                    if (!plan.Enabled)
                        continue;

                    var key = (market.Key, job);
                    if (_running.ContainsKey(key))
                        continue;

                    if (job != MarketJobKind.SyncCatalog && !await IsCatalogReadyAsync(market, ct))
                        continue;

                    if (!_states.TryGetValue(key, out var state))
                    {
                        // اجرای اول: کمی پخش می‌کنیم تا همهٔ کارها در یک لحظه به منابع بیرونی نخورند.
                        state = new JobState { NextDue = now.AddSeconds(_states.Count % 20 * 2) };
                        _states[key] = state;
                    }

                    if (now < state.NextDue || !IsInsideWindow(market, plan, now))
                        continue;

                    _running[key] = RunAsync(market, job, plan, state, ct);
                }
            }
        }

        private async Task RunAsync(MarketSnapshot market, MarketJobKind job, MarketJobPlan plan, JobState state, CancellationToken ct)
        {
            var key = (market.Key, job);
            try
            {
                await _concurrency.WaitAsync(ct);
                try
                {
                    var result = await _runner.RunAsync(market.Key, job, ct);
                    ApplyResult(market, job, plan, state, result);
                }
                finally
                {
                    _concurrency.Release();
                }
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                // خاموش‌شدن
            }
            catch (ClientExposedException ex)
            {
                // قفل برنامه (حالت فقط‌خواندنی): خطای کار نیست؛ کمی بعد دوباره.
                state.NextDue = _time.GetUtcNow() + LockedRetry;
                _logger.LogWarning("Job {Job} for {Market} postponed: {Reason}", job, market.Key, ex.Message);
            }
            catch (Exception ex)
            {
                state.Failures++;
                state.NextDue = _time.GetUtcNow() + Backoff(plan.Interval, state.Failures);
                _logger.LogError(ex, "Job {Job} for {Market} crashed (failure #{Failures}); next attempt {Next:u}", job, market.Key, state.Failures, state.NextDue);
            }
            finally
            {
                _running.TryRemove(key, out _);
            }
        }

        private void ApplyResult(MarketSnapshot market, MarketJobKind job, MarketJobPlan plan, JobState state, MarketJobResult result)
        {
            var now = _time.GetUtcNow();

            switch (result.Status)
            {
                case MarketJobStatus.Completed:
                    state.Failures = 0;
                    state.NextDue = now + plan.Interval;
                    if (job == MarketJobKind.SyncCatalog)
                        _catalogReady.Add(market.Key);

                    _logger.LogInformation("{Job} {Market}: processed={Processed} updated={Updated} noData={NoData} refetched={Refetched} failed={Failed} in {Duration:g}. {Message}",
                        job, market.Key, result.Processed, result.Updated, result.NoData, result.FullyRefetched, result.Failed, result.Duration, result.Message);

                    if (result.Failed > 0)
                        _logger.LogWarning("{Job} {Market} errors (sample): {Errors}", job, market.Key, string.Join(" | ", result.ErrorSamples));
                    break;

                case MarketJobStatus.Skipped:
                    // کاتالوگ بدون Provider یعنی نمادها دستی مدیریت می‌شوند؛ مانع کندل نشود.
                    if (job == MarketJobKind.SyncCatalog)
                        _catalogReady.Add(market.Key);

                    state.NextDue = now + (plan.Interval < SkippedRetry ? plan.Interval : SkippedRetry);
                    _logger.LogDebug("{Job} {Market} skipped: {Message}", job, market.Key, result.Message);
                    break;

                default:
                    state.Failures++;
                    state.NextDue = now + Backoff(plan.Interval, state.Failures);
                    _logger.LogError("{Job} {Market} failed (#{Failures}): {Message} {Errors}. Next attempt {Next:u}",
                        job, market.Key, state.Failures, result.Message, string.Join(" | ", result.ErrorSamples), state.NextDue);
                    break;
            }
        }

        /// <summary>backoff نمایی با سقف یک ساعت؛ حداقل ۳۰ ثانیه تا کار شکست‌خورده بی‌وقفه تکرار نشود.</summary>
        private static TimeSpan Backoff(TimeSpan interval, int failures)
        {
            var baseDelay = interval < TimeSpan.FromSeconds(30) ? TimeSpan.FromSeconds(30) : interval;
            var factor = Math.Pow(2, Math.Min(failures, 6));
            var delay = TimeSpan.FromTicks((long)Math.Min(baseDelay.Ticks * factor, MaxBackoff.Ticks));
            return delay > MaxBackoff ? MaxBackoff : delay;
        }

        /// <summary>
        /// آیا بازار نماد دارد یا فهرستش در همین اجرا همگام شده؟ (بعد از ری‌استارت، نمادهای قبلی دیتابیس کافی‌اند.)
        /// فقط وقتی «آماده نیست» دیتابیس پرسیده می‌شود و بعد از آماده‌شدن دیگر پرسیده نمی‌شود.
        /// </summary>
        private async Task<bool> IsCatalogReadyAsync(MarketSnapshot market, CancellationToken ct)
        {
            if (_catalogReady.Contains(market.Key))
                return true;

            // پاسخ «آماده نیست» را کوتاه‌مدت کش می‌کنیم تا هر ۵ ثانیه فهرست نمادها از دیتابیس خوانده نشود.
            var now = _time.GetUtcNow();
            if (_catalogCheckedAt.TryGetValue(market.Key, out var checkedAt) && now - checkedAt < _options.Value.SettingsCacheDuration)
                return false;
            _catalogCheckedAt[market.Key] = now;

            using var scope = _scopes.CreateScope();
            var symbols = await scope.ServiceProvider.GetRequiredService<IMarketSymbolStore>().GetByMarketAsync(market.Id, onlyActive: true, ct);
            if (symbols.Count == 0)
                return false;

            _catalogReady.Add(market.Key);
            return true;
        }

        private bool IsInsideWindow(MarketSnapshot market, MarketJobPlan plan, DateTimeOffset nowUtc)
        {
            if (plan.WindowFrom is null || plan.WindowTo is null)
                return true;

            var local = TimeZoneInfo.ConvertTime(nowUtc, market.TimeZone);

            // بورس ایران: پنجشنبه و جمعه تعطیل است (تعطیلات رسمی را فعلاً بررسی نمی‌کنیم؛ کار روی روز تعطیل فقط داده‌ٔ تکراری می‌گیرد).
            if (market.Calendar == TradingCalendarKind.IranStockWeek && local.DayOfWeek is DayOfWeek.Thursday or DayOfWeek.Friday)
                return false;

            var t = TimeOnly.FromDateTime(local.DateTime);
            return plan.WindowFrom <= plan.WindowTo
                ? t >= plan.WindowFrom && t <= plan.WindowTo
                : t >= plan.WindowFrom || t <= plan.WindowTo; // پنجرهٔ عبوری از نیمه‌شب
        }

        private async Task<List<MarketSnapshot>> LoadSnapshotAsync(CancellationToken ct)
        {
            using var scope = _scopes.CreateScope();
            var sp = scope.ServiceProvider;
            var markets = await sp.GetRequiredService<IMarketDefinitionStore>().GetAllAsync(ct);
            var settingsStore = sp.GetRequiredService<IMarketSettingStore>();
            var providers = sp.GetServices<IMarketDataProvider>().ToList();

            var list = new List<MarketSnapshot>();
            foreach (var market in markets.Where(m => m.IsActive))
            {
                TimeZoneInfo tz;
                try { tz = TimeZoneInfo.FindSystemTimeZoneById(market.TimeZoneId); }
                catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
                {
                    _logger.LogError("Market {Market} has an unknown time zone '{Zone}'; market skipped", market.Key, market.TimeZoneId);
                    continue;
                }

                var settings = (await settingsStore.GetByScopeAsync(MarketSettingScopes.ForMarket(market.Key), ct))
                    .ToDictionary(s => s.Key, s => s.Value);
                var capabilities = providers.Where(p => p.Descriptor.MarketKey == market.Key)
                    .SelectMany(p => ProviderCapabilities.Of(p)).ToHashSet();

                list.Add(new MarketSnapshot(market.Id, market.Key, market.CalendarKind, tz, settings, capabilities));
            }

            return list;
        }

        private sealed class JobState
        {
            public DateTimeOffset NextDue { get; set; }
            public int Failures { get; set; }
        }

        private sealed record MarketSnapshot(
            int Id, string Key, TradingCalendarKind Calendar, TimeZoneInfo TimeZone,
            IReadOnlyDictionary<string, string> Settings, HashSet<ProviderCapability> Capabilities);
    }
}
