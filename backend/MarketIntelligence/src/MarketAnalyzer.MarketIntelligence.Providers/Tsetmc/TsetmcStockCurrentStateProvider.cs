using System.Globalization;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// آخرین قیمت و وضعیت نمادهای بورس از TradeLastDay، همراه با محدودهٔ مجاز قیمت از StaticThresholds.
    /// فقط نمادهای فعال کاتالوگ برمی‌گردند؛ ردیف نماد ناشناخته بی‌صدا رد می‌شود و شمرده می‌شود.
    /// </summary>
    public sealed class TsetmcStockCurrentStateProvider : ICurrentStateProvider
    {
        private readonly TsetmcWebServiceClient _client;
        private readonly TseSymbolDirectory _directory;
        private readonly ILogger<TsetmcStockCurrentStateProvider> _logger;
        private readonly TimeZoneInfo _marketZone = TimeZoneInfo.FindSystemTimeZoneById(TseMarket.TimeZoneId);

        /// <summary>سازنده.</summary>
        public TsetmcStockCurrentStateProvider(TsetmcWebServiceClient client, TseSymbolDirectory directory, ILogger<TsetmcStockCurrentStateProvider> logger)
        {
            _client = client;
            _directory = directory;
            _logger = logger;
        }

        /// <inheritdoc />
        public ProviderDescriptor Descriptor { get; } = TseMarket.Descriptor(TseMarket.TsetmcProviderKey);

        /// <inheritdoc />
        public async Task<FetchResult<IReadOnlyList<SymbolStateInfo>>> FetchStatesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var map = await _directory.GetInsCodeMapAsync(cancellationToken);
                if (map.Count == 0)
                    return FetchResult<IReadOnlyList<SymbolStateInfo>>.Failed("Stock catalog is empty; sync symbols before current-state ingestion.");

                var trades = await _client.GetTradeLastDayAsync(cancellationToken);
                if (trades.Count == 0)
                    return FetchResult<IReadOnlyList<SymbolStateInfo>>.SuccessEmpty();

                var thresholds = new Dictionary<string, (decimal? Min, decimal? Max)>();
                foreach (var t in await _client.GetStaticThresholdsAsync(cancellationToken))
                {
                    var code = XmlValue.Str(t, "InsCode");
                    if (code != null)
                        thresholds[code] = (XmlValue.Dec(t, "PSGelStaMin"), XmlValue.Dec(t, "PSGelStaMax"));
                }

                var states = new List<SymbolStateInfo>();
                var unknown = 0;
                var badTime = 0;

                foreach (var row in trades)
                {
                    var insCode = XmlValue.Str(row, "InsCode");
                    if (insCode == null || !map.TryGetValue(insCode, out var symbolKey))
                    {
                        unknown++;
                        continue;
                    }

                    if (!TryParseSourceTime(XmlValue.Str(row, "DEven"), XmlValue.Int(row, "HEven"), out var sourceUtc))
                    {
                        badTime++;
                        continue;
                    }

                    thresholds.TryGetValue(insCode, out var limits);

                    states.Add(new SymbolStateInfo
                    {
                        SymbolKey = symbolKey,
                        LastPrice = XmlValue.Dec(row, "PDrCotVal"),
                        ClosePrice = XmlValue.Dec(row, "PClosing"),
                        OpenPrice = XmlValue.Dec(row, "PriceFirst"),
                        HighPrice = XmlValue.Dec(row, "PriceMax"),
                        LowPrice = XmlValue.Dec(row, "PriceMin"),
                        PreviousClosePrice = XmlValue.Dec(row, "PriceYesterday"),
                        PriceLimitMin = limits.Min,
                        PriceLimitMax = limits.Max,
                        Volume = XmlValue.Dec(row, "QTotTran5J"),
                        Value = XmlValue.Dec(row, "QTotCap"),
                        TradeCount = XmlValue.Long(row, "ZTotTran"),
                        SourceTimestampUtc = sourceUtc
                    });
                }

                if (badTime > 0)
                    _logger.LogWarning("Skipped {Count} TSETMC trade rows with unparsable DEven/HEven", badTime);
                _logger.LogDebug("TSETMC current state: {Mapped} mapped, {Unknown} not in catalog", states.Count, unknown);

                return states.Count == 0
                    ? FetchResult<IReadOnlyList<SymbolStateInfo>>.SuccessEmpty()
                    : FetchResult<IReadOnlyList<SymbolStateInfo>>.Success(states, states.Count);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "TSETMC current state fetch failed");
                return FetchResult<IReadOnlyList<SymbolStateInfo>>.Failed($"TSETMC current state fetch failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// DEven به شکل yyyyMMdd و HEven به شکل hhmmss (مثلاً ۹۳۰۰۰ = ۰۹:۳۰:۰۰) به وقت تهران است؛ به UTC تبدیل می‌شود.
        /// </summary>
        private bool TryParseSourceTime(string? dEven, int? hEven, out DateTime utc)
        {
            utc = default;
            if (!DateTime.TryParseExact(dEven, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return false;

            var time = hEven ?? 0;
            var local = date.AddHours(time / 10000).AddMinutes(time / 100 % 100).AddSeconds(time % 100);
            local = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);

            if (_marketZone.IsInvalidTime(local))
                local = local.AddHours(1);

            utc = TimeZoneInfo.ConvertTimeToUtc(local, _marketZone);
            return true;
        }
    }
}
