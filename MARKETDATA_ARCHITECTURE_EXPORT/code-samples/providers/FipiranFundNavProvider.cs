using System.Globalization;
using System.Text.Json;
using MarketAnalyzer.MarketData.Application.Providers;
using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketData.Domain;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketData.Providers
{
    /// <summary>
    /// NAV روزانهٔ صندوق‌ها (صدور، ابطال، آماری) از فیپیران. فهرست صندوق‌ها را از نمادهای فعال همین بازار در دیتابیس می‌خواند
    /// و برای هر صندوق یک درخواست (با مکث <c>RequestDelayMs</c>) می‌زند. اگر ۱۰ صندوق اول همه شکست بخورند کار قطع می‌شود (منبع پایین است)؛
    /// شکست چند صندوق به نتیجهٔ <c>Partial</c> می‌رسد و بقیه ذخیره می‌شوند.
    /// </summary>
    public sealed class FipiranFundNavProvider : IFundNavProvider
    {
        private readonly FipiranClient _client;
        private readonly IMarketDefinitionStore _markets;
        private readonly IMarketSymbolStore _symbols;
        private readonly IMarketSettingStore _settings;
        private readonly TimeProvider _time;
        private readonly ILogger<FipiranFundNavProvider> _logger;

        /// <summary>سازنده.</summary>
        public FipiranFundNavProvider(
            FipiranClient client, IMarketDefinitionStore markets, IMarketSymbolStore symbols, IMarketSettingStore settings, TimeProvider time, ILogger<FipiranFundNavProvider> logger)
        {
            _client = client;
            _markets = markets;
            _symbols = symbols;
            _settings = settings;
            _time = time;
            _logger = logger;
        }

        /// <inheritdoc />
        public ProviderDescriptor Descriptor { get; } = Markets.Fund(FipiranClient.ProviderKey, isDefault: true);

        /// <inheritdoc />
        public async Task<FetchResult<IReadOnlyList<FundNavInfo>>> FetchFundNavsAsync(CancellationToken cancellationToken)
        {
            var market = await _markets.GetByKeyAsync(Descriptor.MarketKey, cancellationToken);
            if (market is null)
                return FetchResult<IReadOnlyList<FundNavInfo>>.Failed($"Market '{Descriptor.MarketKey}' does not exist yet (bootstrap has not run).");

            var funds = await _symbols.GetByMarketAsync(market.Id, onlyActive: true, cancellationToken);
            if (funds.Count == 0)
                return FetchResult<IReadOnlyList<FundNavInfo>>.SuccessEmpty();

            var now = _time.GetUtcNow();
            var schedule = (await _settings.GetByScopeAsync(MarketSettingScopes.ForMarket(Descriptor.MarketKey), cancellationToken))
                .Where(x => x.Key.StartsWith("NavRetry:", StringComparison.Ordinal))
                .ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal);

            var delay = TimeSpan.FromMilliseconds(await _client.GetIntSettingAsync(FipiranClient.RequestDelayMsKey, 1000, cancellationToken));
            var historyDays = await _client.GetIntSettingAsync(FipiranClient.NavHistoryDaysKey, 0, cancellationToken);
            var oldest = historyDays > 0 ? DateOnly.FromDateTime(_time.GetUtcNow().UtcDateTime.AddDays(-historyDays)) : DateOnly.MinValue;

            var all = new List<FundNavInfo>();
            var failures = 0;
            var deferredRetries = 0;
            var errors = new List<string>();

            foreach (var fund in funds)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var stateKey = $"NavRetry:{fund.Id}";
                var prior = schedule.TryGetValue(stateKey, out var stored) ? stored.Split('|') : Array.Empty<string>();
                var failureCount = prior.Length == 2 && int.TryParse(prior[1], NumberStyles.None, CultureInfo.InvariantCulture, out var oldFailures) ? oldFailures : 0;
                if (prior.Length == 2 && DateTimeOffset.TryParse(prior[0], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var nextAttempt) && nextAttempt > now)
                {
                    if (failureCount > 0)
                        deferredRetries++;
                    continue;
                }

                try
                {
                    var body = await _client.GetNavChartAsync(fund.SymbolKey, cancellationToken);
                    all.AddRange(Parse(fund.SymbolKey, body, oldest));
                    var nextDailyRun = now.AddDays(1).ToString("O", CultureInfo.InvariantCulture);
                    await _settings.SetAsync(MarketSettingScopes.ForMarket(Descriptor.MarketKey), stateKey, $"{nextDailyRun}|0", "fipiran-nav", cancellationToken);
                }
                catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or FormatException or InvalidOperationException or OverflowException)
                {
                    if (ex is TaskCanceledException && cancellationToken.IsCancellationRequested)
                        throw;

                    failures++;
                    errors.Add($"{fund.SymbolKey}: {ex.Message}");
                    _logger.LogWarning(ex, "Fipiran NAV fetch failed for fund {Fund}", fund.SymbolKey);
                    var nextFailureCount = failureCount + 1;
                    var retryDelay = TimeSpan.FromMinutes(30);
                    var retryAt = now.Add(retryDelay).ToString("O", CultureInfo.InvariantCulture);
                    await _settings.SetAsync(MarketSettingScopes.ForMarket(Descriptor.MarketKey), stateKey,
                        $"{retryAt}|{nextFailureCount}", "fipiran-nav", cancellationToken);
                }

                if (delay > TimeSpan.Zero)
                    await Task.Delay(delay, _time, cancellationToken);
            }

            if (all.Count == 0)
                return failures > 0
                    ? FetchResult<IReadOnlyList<FundNavInfo>>.Failed($"All {failures} fund requests failed. Errors: {string.Join(" | ", errors)}")
                    : deferredRetries > 0
                        ? FetchResult<IReadOnlyList<FundNavInfo>>.Partial(all, 0, $"Retry pending for {deferredRetries} funds; individual retry times are stored in MarketSettings.")
                    : FetchResult<IReadOnlyList<FundNavInfo>>.SuccessEmpty();

            return failures == 0 && deferredRetries == 0
                ? FetchResult<IReadOnlyList<FundNavInfo>>.Success(all, all.Count)
                : FetchResult<IReadOnlyList<FundNavInfo>>.Partial(all, all.Count, $"{failures} funds failed; {deferredRetries} fund retries are pending. Errors: {string.Join(" | ", errors)}");
        }

        private static IEnumerable<FundNavInfo> Parse(string key, string body, DateOnly oldest)
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                throw new FormatException("NAV chart response is not an array.");

            // یک روز دوبار آمده باشد آخری می‌ماند.
            var byDate = new Dictionary<DateOnly, FundNavInfo>();
            foreach (var row in doc.RootElement.EnumerateArray())
            {
                if (!row.TryGetProperty("date", out var d) || d.ValueKind != JsonValueKind.String)
                    throw new FormatException("NAV row has no 'date'.");

                var date = DateOnly.FromDateTime(d.GetDateTime());
                if (date < oldest)
                    continue;

                byDate[date] = new FundNavInfo
                {
                    SymbolKey = key,
                    Date = date,
                    IssuanceNav = Number(row, "issueNav"),
                    CancellationNav = Number(row, "cancelNav"),
                    StatisticalNav = Number(row, "statisticalNav")
                };
            }

            return byDate.Values;
        }

        private static decimal? Number(JsonElement row, string name)
            => row.TryGetProperty(name, out var v) && v.ValueKind is JsonValueKind.Number or JsonValueKind.String
                ? JsonNumber.ReadDecimal(v)
                : null;
    }
}
