using System.Globalization;
using System.Text.Json;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// کلاینت HTTP کندل نهایت‌نگر (قالب TradingView: آرایه‌های t, o, h, l, c, v). فقط دریافت و تبدیل می‌کند؛
    /// هیچ خطایی بلعیده نمی‌شود و نتیجه همیشه یکی از حالت‌های <see cref="FetchStatus"/> است
    /// (کد قدیمی در هر خطا لیست خالی برمی‌گرداند و شکست را از «داده نیست» جدا نمی‌کرد).
    /// </summary>
    public sealed class NahayatnegarCandleClient
    {
        /// <summary>کلید Provider.</summary>
        public const string ProviderKey = "nahayatnegar";

        /// <summary>کلید تنظیم الگوی آدرس کندل.</summary>
        public const string CandleChartUrlKey = "CandleChartUrl";

        /// <summary>
        /// نوع تعدیل نهایت‌نگر. در کد قدیمی ثابت «3» بود و هم به نماد چسبانده می‌شد و هم در @AdjustmentType می‌رفت؛
        /// همان رفتار حفظ شده است (سری تعدیل‌شده).
        /// </summary>
        private const string AdjustmentType = "3";

        private readonly HttpClient _http;
        private readonly IProviderSettings _settings;
        private readonly TimeProvider _time;
        private readonly ILogger<NahayatnegarCandleClient> _logger;

        /// <summary>سازنده.</summary>
        public NahayatnegarCandleClient(HttpClient http, IProviderSettings settings, TimeProvider time, ILogger<NahayatnegarCandleClient> logger)
        {
            _http = http;
            _settings = settings;
            _time = time;
            _logger = logger;
        }

        /// <summary>کندل‌های یک نماد را می‌گیرد.</summary>
        public async Task<FetchResult<CandleBatch>> FetchAsync(CandleRequest request, CancellationToken cancellationToken)
        {
            var template = await _settings.GetAsync(ProviderKey, CandleChartUrlKey, cancellationToken);
            if (string.IsNullOrWhiteSpace(template))
                return FetchResult<CandleBatch>.Failed($"Setting '{CandleChartUrlKey}' of provider '{ProviderKey}' is not configured.");

            var now = _time.GetUtcNow();
            var from = now - GetLookback(request);
            var url = BuildUrl(template, request, from, now);

            string body;
            try
            {
                using var response = await _http.GetAsync(url, cancellationToken);
                if (!response.IsSuccessStatusCode)
                    return FetchResult<CandleBatch>.Failed($"Nahayatnegar returned HTTP {(int)response.StatusCode} for {request.SymbolKey}.");

                body = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                // TaskCanceledException بدون لغو درخواست یعنی timeout.
                return FetchResult<CandleBatch>.Failed($"Nahayatnegar request failed for {request.SymbolKey}: {ex.Message}", ex);
            }

            try
            {
                return Parse(body, request);
            }
            catch (Exception ex) when (ex is JsonException or FormatException or InvalidOperationException or OverflowException)
            {
                _logger.LogWarning(ex, "Unparseable Nahayatnegar response for {Symbol}", request.SymbolKey);
                return FetchResult<CandleBatch>.Failed($"Unparseable Nahayatnegar response for {request.SymbolKey}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ساخت آدرس. مقدارها URL-encode می‌شوند تا نمادِ دریافتی از منبع بیرونی نتواند ساختار آدرس را تغییر دهد (تزریق پارامتر).
        /// </summary>
        private static string BuildUrl(string template, CandleRequest request, DateTimeOffset from, DateTimeOffset to)
        {
            var symbol = request.SymbolKey + AdjustmentType;

            return template
                .Replace("@Symbol", Uri.EscapeDataString(symbol))
                .Replace("@Resolution", MapResolution(request.Timeframe))
                .Replace("@From", from.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture))
                .Replace("@To", to.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture))
                .Replace("@Type", GetInstrumentType(request.SymbolKey))
                .Replace("@AdjustmentType", AdjustmentType);
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
                    return FetchResult<CandleBatch>.Failed($"Nahayatnegar status '{s}' for {request.SymbolKey}.");
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
                    OpenTimeUtc = DateTimeOffset.FromUnixTimeSeconds(ReadLong(t[i])).UtcDateTime,
                    Open = ReadDecimal(o[i]),
                    High = ReadDecimal(h[i]),
                    Low = ReadDecimal(l[i]),
                    Close = ReadDecimal(c[i]),
                    Volume = v.HasValue ? ReadDecimal(v.Value[i]) : null
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

        /// <summary>عدد ممکن است عددی یا رشته‌ای بیاید (کد قدیمی با Convert.ToDecimal(string) می‌خواند).</summary>
        private static decimal ReadDecimal(JsonElement e) => e.ValueKind switch
        {
            JsonValueKind.Number => e.GetDecimal(),
            JsonValueKind.String => decimal.Parse(e.GetString()!, NumberStyles.Float, CultureInfo.InvariantCulture),
            _ => throw new FormatException($"Unexpected JSON value kind {e.ValueKind} for a number.")
        };

        private static long ReadLong(JsonElement e) => e.ValueKind switch
        {
            JsonValueKind.Number => e.TryGetInt64(out var l) ? l : (long)e.GetDouble(),
            JsonValueKind.String => (long)double.Parse(e.GetString()!, CultureInfo.InvariantCulture),
            _ => throw new FormatException($"Unexpected JSON value kind {e.ValueKind} for a timestamp.")
        };

        /// <summary>نگاشت تایم‌فریم به Resolution نهایت‌نگر (همان نگاشت قدیمی).</summary>
        private static string MapResolution(Timeframe timeframe) => timeframe switch
        {
            Timeframe.FiveMinutes => "5m",
            Timeframe.FifteenMinutes => "15m",
            Timeframe.OneHour => "1",
            Timeframe.FourHours => "4h",
            Timeframe.Daily => "1D",
            Timeframe.Weekly => "1W",
            Timeframe.Mountly => "1M",
            _ => throw new NotSupportedException($"Timeframe {timeframe} is not supported by Nahayatnegar.")
        };

        /// <summary>حق تقدم (IRR...) نوع rights و بقیه stock است (همان قاعدهٔ قدیمی).</summary>
        private static string GetInstrumentType(string symbolKey)
            => symbolKey.StartsWith("IRR", StringComparison.Ordinal) ? "rights" : "stock";
    }
}
