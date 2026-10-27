using System.Globalization;
using System.Text.Json;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// NAV روزانهٔ صندوق‌ها (صدور، ابطال، آماری) از فیپیران. فهرست صندوق‌ها را از نمادهای فعال همین بازار در دیتابیس می‌خواند
    /// و برای هر صندوق یک درخواست (با مکث <c>RequestDelayMs</c>) می‌زند. اگر ۱۰ صندوق اول همه شکست بخورند کار قطع می‌شود (منبع پایین است)؛
    /// شکست چند صندوق به نتیجهٔ <c>Partial</c> می‌رسد و بقیه ذخیره می‌شوند.
    /// </summary>
    public sealed class FipiranFundNavProvider : IFundNavProvider
    {
        private const int AbortAfterConsecutiveFailures = 10;

        private readonly FipiranClient _client;
        private readonly IMarketDefinitionStore _markets;
        private readonly IMarketSymbolStore _symbols;
        private readonly TimeProvider _time;
        private readonly ILogger<FipiranFundNavProvider> _logger;

        /// <summary>سازنده.</summary>
        public FipiranFundNavProvider(
            FipiranClient client, IMarketDefinitionStore markets, IMarketSymbolStore symbols, TimeProvider time, ILogger<FipiranFundNavProvider> logger)
        {
            _client = client;
            _markets = markets;
            _symbols = symbols;
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

            var delay = TimeSpan.FromMilliseconds(await _client.GetIntSettingAsync(FipiranClient.RequestDelayMsKey, 1000, cancellationToken));
            var historyDays = await _client.GetIntSettingAsync(FipiranClient.NavHistoryDaysKey, 0, cancellationToken);
            var oldest = historyDays > 0 ? DateOnly.FromDateTime(_time.GetUtcNow().UtcDateTime.AddDays(-historyDays)) : DateOnly.MinValue;

            var all = new List<FundNavInfo>();
            var failures = 0;
            var consecutive = 0;
            string? firstError = null;

            foreach (var fund in funds)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var body = await _client.GetNavChartAsync(fund.SymbolKey, cancellationToken);
                    all.AddRange(Parse(fund.SymbolKey, body, oldest));
                    consecutive = 0;
                }
                catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or FormatException or InvalidOperationException or OverflowException)
                {
                    if (ex is TaskCanceledException && cancellationToken.IsCancellationRequested)
                        throw;

                    failures++;
                    consecutive++;
                    firstError ??= $"{fund.SymbolKey}: {ex.Message}";
                    _logger.LogWarning(ex, "Fipiran NAV fetch failed for fund {Fund}", fund.SymbolKey);

                    if (consecutive >= AbortAfterConsecutiveFailures && all.Count == 0)
                        return FetchResult<IReadOnlyList<FundNavInfo>>.Failed($"First {consecutive} funds all failed; Fipiran looks down. First error: {firstError}");
                }

                if (delay > TimeSpan.Zero)
                    await Task.Delay(delay, _time, cancellationToken);
            }

            if (all.Count == 0)
                return failures > 0
                    ? FetchResult<IReadOnlyList<FundNavInfo>>.Failed($"All {failures} fund requests failed. First error: {firstError}")
                    : FetchResult<IReadOnlyList<FundNavInfo>>.SuccessEmpty();

            return failures == 0
                ? FetchResult<IReadOnlyList<FundNavInfo>>.Success(all, all.Count)
                : FetchResult<IReadOnlyList<FundNavInfo>>.Partial(all, all.Count, $"{failures} of {funds.Count} funds failed. First error: {firstError}");
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
