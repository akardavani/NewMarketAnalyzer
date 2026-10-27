using System.Globalization;
using System.Text.Json;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// کلاینت قیمت ماهانهٔ مسکن (کیلید). هر ماه یک مقدار می‌آید؛ باز/بالا/پایین/پایانی همان مقدار است و حجم ندارد.
    /// اگر دو نقطه به یک ماه میلادی بیفتند (تعریف ماه در منبع با تقویم میلادی ما فرق داشته باشد) داده بی‌صدا ادغام نمی‌شود،
    /// نتیجه «شکست» با توضیح است تا مشکل دیده شود.
    /// </summary>
    public sealed class KilidCandleClient
    {
        /// <summary>کلید Provider.</summary>
        public const string ProviderKey = "kilid";

        /// <summary>کلید تنظیم الگوی آدرس.</summary>
        public const string CandleChartUrlKey = "CandleChartUrl";

        private readonly HttpClient _http;
        private readonly IProviderSettings _settings;
        private readonly ILogger<KilidCandleClient> _logger;

        /// <summary>سازنده.</summary>
        public KilidCandleClient(HttpClient http, IProviderSettings settings, ILogger<KilidCandleClient> logger)
        {
            _http = http;
            _settings = settings;
            _logger = logger;
        }

        /// <summary>سری ماهانهٔ یک منطقه.</summary>
        public async Task<FetchResult<CandleBatch>> FetchAsync(CandleRequest request, CancellationToken ct)
        {
            if (request.Timeframe != Timeframe.Mountly)
                return FetchResult<CandleBatch>.Failed($"Timeframe {request.Timeframe} is not supported by Kilid (monthly only).");

            var template = await _settings.GetAsync(ProviderKey, CandleChartUrlKey, ct);
            if (string.IsNullOrWhiteSpace(template))
                return FetchResult<CandleBatch>.Failed($"Setting '{CandleChartUrlKey}' of provider '{ProviderKey}' is not configured.");

            var months = request.LastCandlesCount is > 0 ? request.LastCandlesCount.Value + 1 : (request.YearsBack ?? 1) * 12;
            var url = template
                .Replace("@GlobalLocationId", Uri.EscapeDataString(request.SymbolKey))
                .Replace("@DurationMonths", months.ToString(CultureInfo.InvariantCulture));

            string body;
            try
            {
                using var response = await _http.GetAsync(url, ct);
                if (!response.IsSuccessStatusCode)
                    return FetchResult<CandleBatch>.Failed($"Kilid returned HTTP {(int)response.StatusCode} for region {request.SymbolKey}.");

                body = await response.Content.ReadAsStringAsync(ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                return FetchResult<CandleBatch>.Failed($"Kilid request failed for region {request.SymbolKey}: {ex.Message}", ex);
            }

            try
            {
                return Parse(body, request);
            }
            catch (Exception ex) when (ex is JsonException or FormatException or InvalidOperationException or OverflowException)
            {
                _logger.LogWarning(ex, "Unparseable Kilid response for region {Region}", request.SymbolKey);
                return FetchResult<CandleBatch>.Failed($"Unparseable Kilid response for region {request.SymbolKey}: {ex.Message}", ex);
            }
        }

        private static FetchResult<CandleBatch> Parse(string body, CandleRequest request)
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            // نام فیلدها بی‌توجه به حروف بزرگ و کوچک (کد قدیمی هم همین‌طور می‌خواند).
            if (root.ValueKind != JsonValueKind.Object || !TryGet(root, "trends", out var trends) || trends.ValueKind != JsonValueKind.Array)
                return FetchResult<CandleBatch>.Failed($"Kilid response for region {request.SymbolKey} has no 'trends' array.");

            var candles = new List<CandleData>();
            var months = new HashSet<(int Year, int Month)>();

            foreach (var t in trends.EnumerateArray())
            {
                if (!TryGet(t, "date", out var date) || date.ValueKind != JsonValueKind.String
                    || !TryGet(t, "value", out var value) || value.ValueKind is not (JsonValueKind.Number or JsonValueKind.String))
                    throw new FormatException("Kilid trend item misses 'date' or 'value'.");

                var parsed = date.GetDateTime();
                var utc = parsed.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(parsed, DateTimeKind.Utc) : parsed.ToUniversalTime();

                if (!months.Add((utc.Year, utc.Month)))
                    return FetchResult<CandleBatch>.Failed(
                        $"Kilid region {request.SymbolKey} has two points in Gregorian month {utc:yyyy-MM}; the source month boundaries do not match the Gregorian calendar.");

                var v = JsonNumber.ReadDecimal(value);
                candles.Add(new CandleData { OpenTimeUtc = utc, Open = v, High = v, Low = v, Close = v });
            }

            if (candles.Count == 0)
                return FetchResult<CandleBatch>.SuccessEmpty();

            candles = candles.OrderBy(c => c.OpenTimeUtc).ToList();
            if (request.LastCandlesCount is > 0 && candles.Count > request.LastCandlesCount.Value)
                candles = candles.Skip(candles.Count - request.LastCandlesCount.Value).ToList();

            return FetchResult<CandleBatch>.Success(
                new CandleBatch { SymbolKey = request.SymbolKey, Timeframe = request.Timeframe, Candles = candles }, candles.Count);
        }

        private static bool TryGet(JsonElement obj, string name, out JsonElement value)
        {
            foreach (var p in obj.EnumerateObject())
            {
                if (string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    value = p.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}
