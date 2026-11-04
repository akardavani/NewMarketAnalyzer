using System.Globalization;
using System.Text.Json;
using MarketAnalyzer.MarketData.Application.Providers;
using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketData.Domain;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketData.Providers
{
    /// <summary>تاریخچهٔ شاخص‌ها از API رسمی TSETMC (IndexB2).</summary>
    public sealed class TsetmcIndexCandleProvider : IHistoricalCandleProvider
    {
        public const string ProviderKey = "tsetmc-index";

        private static readonly TimeSpan TehranOffset = TimeSpan.FromHours(3.5);
        private readonly HttpClient _http;
        private readonly IProviderSettings _settings;
        private readonly ILogger<TsetmcIndexCandleProvider> _logger;

        public TsetmcIndexCandleProvider(
            HttpClient http,
            IProviderSettings settings,
            ILogger<TsetmcIndexCandleProvider> logger)
        {
            _http = http;
            _settings = settings;
            _logger = logger;
        }

        public ProviderDescriptor Descriptor { get; } = Markets.TseIndex(ProviderKey);

        public async Task<FetchResult<CandleBatch>> FetchCandlesAsync(CandleRequest request, CancellationToken cancellationToken)
        {
            if (request.Timeframe != Timeframe.Daily)
                return FetchResult<CandleBatch>.Failed($"Timeframe {request.Timeframe} is not supported by TSETMC index history (daily only).");

            var insCode = request.ExternalId;
            if (string.IsNullOrWhiteSpace(insCode))
                insCode = request.SymbolKey;
            if (!long.TryParse(insCode, NumberStyles.None, CultureInfo.InvariantCulture, out _))
                return FetchResult<CandleBatch>.Failed($"TSETMC index history requires a numeric InsCode; received '{insCode}'.");

            var template = await _settings.GetAsync(
                TseMarket.TsetmcProviderKey,
                TsetmcSettingDefaults.IndexHistoryUrlKey,
                cancellationToken);
            if (string.IsNullOrWhiteSpace(template))
                return FetchResult<CandleBatch>.Failed("TSETMC index history URL is not configured.");

            var url = template.Replace("@InsCode", Uri.EscapeDataString(insCode));
            string body;
            try
            {
                using var response = await _http.GetAsync(url, cancellationToken);
                if (!response.IsSuccessStatusCode)
                    return FetchResult<CandleBatch>.Failed($"TSETMC index history returned HTTP {(int)response.StatusCode} for {request.SymbolKey}.");
                body = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                return FetchResult<CandleBatch>.Failed($"TSETMC index history request failed for {request.SymbolKey}: {ex.Message}", ex);
            }

            try
            {
                using var document = JsonDocument.Parse(body);
                if (!document.RootElement.TryGetProperty("indexB2", out var rows) || rows.ValueKind != JsonValueKind.Array)
                    return FetchResult<CandleBatch>.Failed($"TSETMC index response for {request.SymbolKey} has no 'indexB2' array.");
                if (rows.GetArrayLength() == 0)
                    return FetchResult<CandleBatch>.SuccessEmpty();

                var byDay = new SortedDictionary<DateTime, CandleData>();
                foreach (var row in rows.EnumerateArray())
                {
                    var dateValue = row.GetProperty("dEven").GetInt32();
                    var date = DateTime.ParseExact(
                        dateValue.ToString(CultureInfo.InvariantCulture),
                        "yyyyMMdd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None);
                    var open = row.GetProperty("xNivInuPbMresIbs").GetDecimal();
                    var close = row.GetProperty("xNivInuClMresIbs").GetDecimal();
                    var high = row.GetProperty("xNivInuPhMresIbs").GetDecimal();
                    var openTimeUtc = new DateTimeOffset(date.AddHours(12), TehranOffset).UtcDateTime;

                    byDay[date] = new CandleData
                    {
                        OpenTimeUtc = openTimeUtc,
                        Open = open,
                        High = Math.Max(high, Math.Max(open, close)),
                        Low = Math.Min(open, close),
                        Close = close
                    };
                }

                var candles = byDay.Values.ToList();
                if (request.LastCandlesCount is > 0 && candles.Count > request.LastCandlesCount.Value)
                    candles = candles.Skip(candles.Count - request.LastCandlesCount.Value).ToList();

                return FetchResult<CandleBatch>.Success(
                    new CandleBatch { SymbolKey = request.SymbolKey, Timeframe = request.Timeframe, Candles = candles },
                    candles.Count);
            }
            catch (Exception ex) when (ex is JsonException or FormatException or InvalidOperationException or OverflowException)
            {
                _logger.LogWarning(ex, "Unparseable TSETMC index response for {Symbol}", request.SymbolKey);
                return FetchResult<CandleBatch>.Failed($"Unparseable TSETMC index response for {request.SymbolKey}: {ex.Message}", ex);
            }
        }
    }
}
