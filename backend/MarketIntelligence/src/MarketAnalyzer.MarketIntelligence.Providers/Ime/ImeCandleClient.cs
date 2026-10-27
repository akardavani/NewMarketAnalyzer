using System.Globalization;
using System.Text.Json;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// کلاینت کندل قراردادهای آتی بورس کالا. پاسخ JSON است با آرایهٔ <c>Data</c> یا <c>rows</c> و فیلدهای
    /// FirstPrice/MaxPrice/MinPrice/LastPrice/TradesVolume/TradesValue و DT (تاریخ شمسی).
    /// برخلاف کد قدیمی، وضعیت (کد قرارداد و...) در فیلد کلاس نگه داشته نمی‌شود (آن حالت مشترک در اجرای موازی خراب می‌شد)،
    /// و خطای شبکه یا پاسخ نامعتبر به «شکست» تبدیل می‌شود، نه «بدون کندل».
    /// </summary>
    public sealed class ImeCandleClient
    {
        /// <summary>کلید Provider.</summary>
        public const string ProviderKey = "ime";

        /// <summary>کلید تنظیم الگوی آدرس.</summary>
        public const string CandleChartUrlKey = "CandleChartUrl";

        private static readonly PersianCalendar Persian = new();
        private static readonly TimeSpan TehranStandardOffset = TimeSpan.FromHours(3.5);

        private readonly HttpClient _http;
        private readonly IProviderSettings _settings;
        private readonly TimeProvider _time;
        private readonly ILogger<ImeCandleClient> _logger;

        /// <summary>سازنده.</summary>
        public ImeCandleClient(HttpClient http, IProviderSettings settings, TimeProvider time, ILogger<ImeCandleClient> logger)
        {
            _http = http;
            _settings = settings;
            _time = time;
            _logger = logger;
        }

        /// <summary>کندل‌های یک قرارداد.</summary>
        public async Task<FetchResult<CandleBatch>> FetchAsync(CandleRequest request, CancellationToken ct)
        {
            if (request.Timeframe != Timeframe.Daily)
                return FetchResult<CandleBatch>.Failed($"Timeframe {request.Timeframe} is not supported by IME (daily only).");

            var template = await _settings.GetAsync(ProviderKey, CandleChartUrlKey, ct);
            if (string.IsNullOrWhiteSpace(template))
                return FetchResult<CandleBatch>.Failed($"Setting '{CandleChartUrlKey}' of provider '{ProviderKey}' is not configured.");

            var now = _time.GetUtcNow();
            var span = request.LastCandlesCount is > 0
                ? TimeSpan.FromDays(request.LastCandlesCount.Value * 2.0 + 14)
                : TimeSpan.FromDays(365.25 * (request.YearsBack ?? 1));

            // @From و @To تاریخ شمسی‌اند (ارقام لاتین و /)؛ نماد encode می‌شود تا ساختار آدرس را تغییر ندهد.
            var url = template
                .Replace("@Symbol", Uri.EscapeDataString(request.SymbolKey))
                .Replace("@From", ToPersianDate(now - span))
                .Replace("@To", ToPersianDate(now));

            string body;
            try
            {
                using var response = await _http.GetAsync(url, ct);
                if (!response.IsSuccessStatusCode)
                    return FetchResult<CandleBatch>.Failed($"IME returned HTTP {(int)response.StatusCode} for {request.SymbolKey}.");

                body = await response.Content.ReadAsStringAsync(ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                return FetchResult<CandleBatch>.Failed($"IME request failed for {request.SymbolKey}: {ex.Message}", ex);
            }

            try
            {
                return Parse(body, request);
            }
            catch (Exception ex) when (ex is JsonException or FormatException or InvalidOperationException or OverflowException)
            {
                _logger.LogWarning(ex, "Unparseable IME response for {Symbol}", request.SymbolKey);
                return FetchResult<CandleBatch>.Failed($"Unparseable IME response for {request.SymbolKey}: {ex.Message}", ex);
            }
        }

        private static FetchResult<CandleBatch> Parse(string body, CandleRequest request)
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            JsonElement rows = default;
            var found = root.ValueKind == JsonValueKind.Object
                        && (root.TryGetProperty("Data", out rows) || root.TryGetProperty("rows", out rows))
                        && rows.ValueKind == JsonValueKind.Array;
            if (!found)
                return FetchResult<CandleBatch>.Failed($"IME response for {request.SymbolKey} has no 'Data' or 'rows' array.");

            var byDay = new SortedDictionary<DateTime, CandleData>();
            foreach (var row in rows.EnumerateArray())
            {
                var close = Number(row, "LastPrice");
                if (close is null or <= 0)
                    continue; // روزِ بدون معامله (همان قاعدهٔ کد قدیمی)

                if (!row.TryGetProperty("DT", out var dt) || dt.ValueKind != JsonValueKind.String || !TryParseDay(dt.GetString(), out var day))
                    throw new FormatException("IME row has a missing or invalid 'DT' date.");

                // ظهر محل؛ اعتبارسنج آن را به شروع روز تقویم بازار می‌رساند و از مشکل نیمه‌شب/ساعت تابستانی دور می‌ماند.
                var instant = new DateTimeOffset(day.AddHours(12), TehranStandardOffset).UtcDateTime;

                byDay[day] = new CandleData
                {
                    OpenTimeUtc = instant,
                    Open = Number(row, "FirstPrice") ?? 0m,
                    High = Number(row, "MaxPrice") ?? 0m,
                    Low = Number(row, "MinPrice") ?? 0m,
                    Close = close.Value,
                    Volume = Number(row, "TradesVolume"),
                    Value = Number(row, "TradesValue")
                };
            }

            if (byDay.Count == 0)
                return FetchResult<CandleBatch>.SuccessEmpty();

            var list = byDay.Values.ToList();
            if (request.LastCandlesCount is > 0 && list.Count > request.LastCandlesCount.Value)
                list = list.Skip(list.Count - request.LastCandlesCount.Value).ToList();

            return FetchResult<CandleBatch>.Success(
                new CandleBatch { SymbolKey = request.SymbolKey, Timeframe = request.Timeframe, Candles = list }, list.Count);
        }

        private static decimal? Number(JsonElement row, string name)
            => row.TryGetProperty(name, out var v) && v.ValueKind is JsonValueKind.Number or JsonValueKind.String
                ? JsonNumber.ReadDecimal(v)
                : null;

        /// <summary>تاریخ شمسی «yyyy/MM/dd» یا تاریخ میلادی (ISO) را به روز میلادی تبدیل می‌کند.</summary>
        private static bool TryParseDay(string? text, out DateTime day)
        {
            day = default;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            var parts = text.Trim().Split('/');
            if (parts.Length == 3
                && int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var y)
                && int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var m)
                && int.TryParse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture, out var d)
                && y is >= 1200 and <= 1700)
            {
                try
                {
                    day = Persian.ToDateTime(y, m, d, 0, 0, 0, 0);
                    return true;
                }
                catch (ArgumentOutOfRangeException)
                {
                    return false;
                }
            }

            if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            {
                day = parsed.Date;
                return true;
            }

            return false;
        }

        private static string ToPersianDate(DateTimeOffset utc)
        {
            var local = utc.ToOffset(TehranStandardOffset).DateTime;
            return string.Create(CultureInfo.InvariantCulture,
                $"{Persian.GetYear(local):0000}/{Persian.GetMonth(local):00}/{Persian.GetDayOfMonth(local):00}");
        }
    }
}
