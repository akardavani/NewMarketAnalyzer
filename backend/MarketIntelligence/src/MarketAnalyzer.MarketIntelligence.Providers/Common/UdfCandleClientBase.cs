using System.Globalization;
using System.Text.Json;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// پایهٔ کلاینت‌های کندل با قالب TradingView UDF (آرایه‌های t, o, h, l, c و v): نهایت‌نگر، Wallex، Nobitex و Tgju.
    /// فقط ساخت آدرس در کلاس‌های فرزند فرق می‌کند. هیچ خطایی بلعیده نمی‌شود: نتیجه همیشه یکی از حالت‌های
    /// <see cref="FetchStatus"/> است (کد قدیمی هر خطا را به «لیست خالی» تبدیل می‌کرد).
    /// قیمت‌ها <b>با همان واحد منبع</b> ذخیره می‌شوند (تبدیل تومان به ریال حذف شد؛ واحد را CurrencyCode نشان می‌دهد).
    /// </summary>
    public abstract class UdfCandleClientBase
    {
        private readonly HttpClient _http;
        private readonly IProviderSettings _settings;
        private readonly TimeProvider _time;
        private readonly ILogger _logger;

        /// <summary>سازنده.</summary>
        protected UdfCandleClientBase(HttpClient http, IProviderSettings settings, TimeProvider time, ILogger logger)
        {
            _http = http;
            _settings = settings;
            _time = time;
            _logger = logger;
        }

        /// <summary>کلید Provider که تنظیماتش را می‌خواند.</summary>
        protected abstract string SourceKey { get; }

        /// <summary>نام تنظیم الگوی آدرس کندل.</summary>
        protected abstract string UrlSettingName { get; }

        /// <summary>الگوی آدرس را با مقدارهای درخواست پر می‌کند. مقدارهای متغیر باید URL-encode شوند.</summary>
        protected abstract string BuildUrl(string template, CandleRequest request, string resolution, DateTimeOffset from, DateTimeOffset to);

        /// <summary>کندل‌های یک نماد را می‌گیرد.</summary>
        public async Task<FetchResult<CandleBatch>> FetchAsync(CandleRequest request, CancellationToken cancellationToken)
        {
            var template = await _settings.GetAsync(SourceKey, UrlSettingName, cancellationToken);
            if (string.IsNullOrWhiteSpace(template))
                return FetchResult<CandleBatch>.Failed($"Setting '{UrlSettingName}' of provider '{SourceKey}' is not configured.");

            string url;
            try
            {
                var now = _time.GetUtcNow();
                url = BuildUrl(template, request, MapResolution(request.Timeframe), now - GetLookback(request), now);
            }
            catch (NotSupportedException ex)
            {
                return FetchResult<CandleBatch>.Failed(ex.Message, ex);
            }

            string body;
            try
            {
                using var response = await _http.GetAsync(url, cancellationToken);
                if (!response.IsSuccessStatusCode)
                    return FetchResult<CandleBatch>.Failed($"{SourceKey} returned HTTP {(int)response.StatusCode} for {request.SymbolKey}.");

                body = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                // TaskCanceledException بدون لغو درخواست یعنی timeout.
                return FetchResult<CandleBatch>.Failed($"{SourceKey} request failed for {request.SymbolKey}: {ex.Message}", ex);
            }

            try
            {
                return Parse(body, request);
            }
            catch (Exception ex) when (ex is JsonException or FormatException or InvalidOperationException or OverflowException)
            {
                _logger.LogWarning(ex, "Unparseable {Source} response for {Symbol}", SourceKey, request.SymbolKey);
                return FetchResult<CandleBatch>.Failed($"Unparseable {SourceKey} response for {request.SymbolKey}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// بازهٔ زمانی درخواست. برای «N کندل اخیر» روزهای تقویمی بیشتری می‌خواهیم (تعطیلات و آخر هفته‌ها) و بعد از دریافت
        /// فقط N کندل آخر را نگه می‌داریم.
        /// </summary>
        private static TimeSpan GetLookback(CandleRequest request)
        {
            if (request.LastCandlesCount is > 0)
                return TimeSpan.FromDays(request.LastCandlesCount.Value * 2.0 + 14);

            return TimeSpan.FromDays(365.25 * (request.YearsBack ?? 1));
        }

        private FetchResult<CandleBatch> Parse(string body, CandleRequest request)
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
                return FetchResult<CandleBatch>.Failed($"Unexpected response shape for {request.SymbolKey}.");

            // قالب TradingView: s = "ok" | "no_data" | "error". اگر منبع s نفرستد از روی آرایه‌ها قضاوت می‌کنیم.
            if (root.TryGetProperty("s", out var status) && status.ValueKind == JsonValueKind.String)
            {
                var s = status.GetString();
                if (string.Equals(s, "no_data", StringComparison.OrdinalIgnoreCase))
                    return FetchResult<CandleBatch>.SuccessEmpty();
                if (!string.Equals(s, "ok", StringComparison.OrdinalIgnoreCase))
                    return FetchResult<CandleBatch>.Failed($"{SourceKey} status '{s}' for {request.SymbolKey}.");
            }

            if (!root.TryGetProperty("t", out var t) || t.ValueKind != JsonValueKind.Array || t.GetArrayLength() == 0)
                return FetchResult<CandleBatch>.SuccessEmpty();

            var count = t.GetArrayLength();
            var o = RequireArray(root, "o", count);
            var h = RequireArray(root, "h", count);
            var l = RequireArray(root, "l", count);
            var c = RequireArray(root, "c", count);
            var v = root.TryGetProperty("v", out var vArray) && vArray.ValueKind == JsonValueKind.Array && vArray.GetArrayLength() == count
                ? vArray
                : (JsonElement?)null;

            var candles = new List<CandleData>(count);
            for (var i = 0; i < count; i++)
            {
                candles.Add(new CandleData
                {
                    OpenTimeUtc = DateTimeOffset.FromUnixTimeSeconds(JsonNumber.ReadLong(t[i])).UtcDateTime,
                    Open = JsonNumber.ReadDecimal(o[i]),
                    High = JsonNumber.ReadDecimal(h[i]),
                    Low = JsonNumber.ReadDecimal(l[i]),
                    Close = JsonNumber.ReadDecimal(c[i]),
                    Volume = v.HasValue ? JsonNumber.ReadDecimal(v.Value[i]) : null
                });
            }

            // فقط N کندل آخر (ترتیب و حذف تکراری را اعتبارسنج انجام می‌دهد؛ اینجا فقط برش).
            var ordered = candles.OrderBy(x => x.OpenTimeUtc).ToList();
            if (request.LastCandlesCount is > 0 && ordered.Count > request.LastCandlesCount.Value)
                ordered = ordered.Skip(ordered.Count - request.LastCandlesCount.Value).ToList();

            return FetchResult<CandleBatch>.Success(
                new CandleBatch { SymbolKey = request.SymbolKey, Timeframe = request.Timeframe, Candles = ordered },
                ordered.Count);
        }

        private static JsonElement RequireArray(JsonElement root, string name, int expectedLength)
        {
            if (!root.TryGetProperty(name, out var array) || array.ValueKind != JsonValueKind.Array || array.GetArrayLength() != expectedLength)
                throw new FormatException($"Array '{name}' is missing or its length differs from 't'.");
            return array;
        }

        /// <summary>نگاشت تایم‌فریم به Resolution قالب UDF (همان نگاشت قدیمی).</summary>
        protected static string MapResolution(Timeframe timeframe) => timeframe switch
        {
            Timeframe.FiveMinutes => "5m",
            Timeframe.FifteenMinutes => "15m",
            Timeframe.OneHour => "1",
            Timeframe.FourHours => "4h",
            Timeframe.Daily => "1D",
            Timeframe.Weekly => "1W",
            Timeframe.Mountly => "1M",
            _ => throw new NotSupportedException($"Timeframe {timeframe} is not supported.")
        };

        /// <summary>تبدیل زمان به ثانیهٔ یونیکس به‌صورت رشته برای جایگزینی در آدرس.</summary>
        protected static string Unix(DateTimeOffset value) => value.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
    }
}
