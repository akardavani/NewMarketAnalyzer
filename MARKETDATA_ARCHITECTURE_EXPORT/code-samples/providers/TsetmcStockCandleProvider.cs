using System.Globalization;
using MarketAnalyzer.MarketData.Application.Providers;
using MarketAnalyzer.MarketData.Domain;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketData.Providers;

/// <summary>
/// Daily TSETMC share history using the Export-txt endpoint and the performance-adjustment
/// calculation used by pytse_client. The returned candles are already adjusted.
/// </summary>
public sealed class TsetmcStockCandleProvider : IHistoricalCandleProvider
{
    public const string ProviderKey = TseMarket.TsetmcProviderKey;
    private static readonly TimeSpan TehranOffset = TimeSpan.FromHours(3.5);
    private readonly HttpClient _http;
    private readonly IProviderSettings _settings;
    private readonly ILogger<TsetmcStockCandleProvider> _logger;

    public TsetmcStockCandleProvider(
        HttpClient http,
        IProviderSettings settings,
        ILogger<TsetmcStockCandleProvider> logger)
    {
        _http = http;
        _settings = settings;
        _logger = logger;
    }

    public ProviderDescriptor Descriptor { get; } = TseMarket.Descriptor(
        ProviderKey,
        AdjustmentMode.AlreadyAdjusted);

    public async Task<FetchResult<CandleBatch>> FetchCandlesAsync(
        CandleRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Timeframe != Timeframe.Daily)
            return FetchResult<CandleBatch>.Failed("TSETMC stock history supports daily candles only.");

        var insCode = request.ExternalId;
        if (string.IsNullOrWhiteSpace(insCode) || !long.TryParse(insCode, NumberStyles.None, CultureInfo.InvariantCulture, out _))
            return FetchResult<CandleBatch>.Failed($"TSETMC stock history requires a numeric InsCode for {request.SymbolKey}.");

        var template = await _settings.GetAsync(
            ProviderKey,
            TsetmcSettingDefaults.StockHistoryUrlKey,
            cancellationToken);
        if (string.IsNullOrWhiteSpace(template))
            return FetchResult<CandleBatch>.Failed("TSETMC stock history URL is not configured.");

        var url = template.Replace("@InsCode", Uri.EscapeDataString(insCode), StringComparison.Ordinal);
        string body;
        try
        {
            using var response = await _http.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return FetchResult<CandleBatch>.Failed($"TSETMC stock history returned HTTP {(int)response.StatusCode} for {request.SymbolKey}.");
            body = await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return FetchResult<CandleBatch>.Failed($"TSETMC stock history request failed for {request.SymbolKey}: {ex.Message}", ex);
        }

        try
        {
            var parsed = ParseCsv(body);
            if (parsed.Count == 0)
                return FetchResult<CandleBatch>.SuccessEmpty();

            ApplyPytsePerformanceAdjustment(parsed);
            var candles = parsed
                .Where(row => row.Close > 0 && row.Volume > 0)
                .Select(row => new CandleData
                {
                    OpenTimeUtc = new DateTimeOffset(row.Date.AddHours(12), TehranOffset).UtcDateTime,
                    Open = row.Open,
                    High = row.High,
                    Low = row.Low,
                    Close = row.Close,
                    Volume = row.Volume,
                    Value = row.Value
                })
                .ToList();

            if (request.LastCandlesCount is > 0 && candles.Count > request.LastCandlesCount.Value)
                candles = candles.Skip(candles.Count - request.LastCandlesCount.Value).ToList();

            return candles.Count == 0
                ? FetchResult<CandleBatch>.SuccessEmpty()
                : FetchResult<CandleBatch>.Success(
                    new CandleBatch { SymbolKey = request.SymbolKey, Timeframe = request.Timeframe, Candles = candles },
                    candles.Count);
        }
        catch (Exception ex) when (ex is FormatException or InvalidDataException or OverflowException or DivideByZeroException)
        {
            _logger.LogWarning(ex, "Unparseable TSETMC stock history for {Symbol}", request.SymbolKey);
            return FetchResult<CandleBatch>.Failed($"Unparseable TSETMC stock history for {request.SymbolKey}: {ex.Message}", ex);
        }
    }

    private static List<DailyRow> ParseCsv(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return [];

        using var reader = new StringReader(body.TrimStart('\uFEFF'));
        var headerLine = reader.ReadLine();
        if (string.IsNullOrWhiteSpace(headerLine))
            return [];

        var headers = SplitCsvLine(headerLine).Select(x => x.Trim()).ToArray();
        var dateIndex = IndexOf(headers, "<DTYYYYMMDD>");
        var openIndex = IndexOf(headers, "<FIRST>");
        var highIndex = IndexOf(headers, "<HIGH>");
        var lowIndex = IndexOf(headers, "<LOW>");
        var closeIndex = IndexOf(headers, "<LAST>");
        var volumeIndex = IndexOf(headers, "<VOL>");
        var adjustedCloseIndex = IndexOf(headers, "<CLOSE>");
        var valueIndex = IndexOf(headers, "<VALUE>");
        var yesterdayIndex = IndexOf(headers, "<OPEN>");

        if (new[] { dateIndex, openIndex, highIndex, lowIndex, closeIndex, volumeIndex, adjustedCloseIndex, yesterdayIndex }.Any(i => i < 0))
            throw new InvalidDataException("The export is missing one or more required TSETMC history columns.");

        var rows = new List<DailyRow>();
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var fields = SplitCsvLine(line);
            if (fields.Count < headers.Length) continue;
            rows.Add(new DailyRow
            {
                Date = DateTime.ParseExact(fields[dateIndex], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None),
                Open = Number(fields[openIndex]),
                High = Number(fields[highIndex]),
                Low = Number(fields[lowIndex]),
                Close = Number(fields[closeIndex]),
                Volume = Number(fields[volumeIndex]),
                AdjustedClose = Number(fields[adjustedCloseIndex]),
                Value = valueIndex >= 0 ? Number(fields[valueIndex]) : null,
                Yesterday = Number(fields[yesterdayIndex])
            });
        }

        // Export-txt returns newest first; pytse_client reverses to chronological order before adjustment.
        rows.Reverse();
        return rows;
    }

    private static void ApplyPytsePerformanceAdjustment(List<DailyRow> rows)
    {
        var breaks = new List<int>();
        for (var i = 1; i < rows.Count; i++)
            if (rows[i - 1].AdjustedClose != rows[i].Yesterday)
                breaks.Add(i);
        if (breaks.Count > 0) breaks.RemoveAt(0);

        var cumulativeFactors = new decimal[breaks.Count];
        var factor = 1m;
        for (var j = breaks.Count - 1; j >= 0; j--)
        {
            var index = breaks[j];
            var previousAdjustedClose = rows[index - 1].AdjustedClose;
            if (previousAdjustedClose == 0)
                throw new DivideByZeroException("A corporate-action boundary has a zero adjusted close.");
            factor *= rows[index].Yesterday / previousAdjustedClose;
            cumulativeFactors[j] = factor;
        }

        for (var j = 0; j < breaks.Count; j++)
        {
            var start = j == 0 ? 0 : breaks[j - 1];
            var end = breaks[j] - 1;
            for (var i = start; i <= end; i++)
            {
                var row = rows[i];
                var ratio = cumulativeFactors[j];
                row.Open = decimal.Round(row.Open * ratio, 0, MidpointRounding.ToEven);
                row.High = decimal.Round(row.High * ratio, 0, MidpointRounding.ToEven);
                row.Low = decimal.Round(row.Low * ratio, 0, MidpointRounding.ToEven);
                row.Close = decimal.Round(row.Close * ratio, 0, MidpointRounding.ToEven);
            }
        }
    }

    private static int IndexOf(IReadOnlyList<string> headers, string name)
    {
        for (var i = 0; i < headers.Count; i++)
            if (string.Equals(headers[i], name, StringComparison.OrdinalIgnoreCase)) return i;
        return -1;
    }

    private static decimal Number(string value)
        => decimal.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture);

    private static List<string> SplitCsvLine(string line)
    {
        var values = new List<string>();
        var current = new System.Text.StringBuilder();
        var quoted = false;
        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];
            if (ch == '"')
            {
                if (quoted && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else quoted = !quoted;
            }
            else if (ch == ',' && !quoted)
            {
                values.Add(current.ToString());
                current.Clear();
            }
            else current.Append(ch);
        }
        values.Add(current.ToString());
        return values;
    }

    private sealed class DailyRow
    {
        public DateTime Date { get; init; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; init; }
        public decimal AdjustedClose { get; init; }
        public decimal? Value { get; init; }
        public decimal Yesterday { get; init; }
    }
}
