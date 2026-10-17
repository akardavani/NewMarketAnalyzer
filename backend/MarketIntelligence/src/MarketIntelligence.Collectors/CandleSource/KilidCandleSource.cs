using MarketAnalyzer.Collector;
using MarketIntelligence.Collectors;
using MarketIntelligence.Domain;
using Microsoft.Extensions.Options;
using System.Text.Json;

public class KilidCandleSource : ICandleSource
{
    private readonly CandleSettings? _settings;
    private readonly IDataFetcher _fetcher;

    public MarketType MarketType => MarketType.RealEstate;
    public SourceType SourceType => SourceType.Kilid;

    public KilidCandleSource(
        IOptions<UrlSettings> settings,
        IDataFetcher fetcher)
    {
        _settings = settings.Value.KilidSettings;
        _fetcher = fetcher;

        if (_settings != null)
        {
            if (string.IsNullOrWhiteSpace(_settings.SymbolUrl))
            {
                _settings = null;
            }
        }
    }

    public async Task<List<UnifiedCandleRecord>> FetchCandlesAsync(
        string symbol,
        Timeframe timeframe,
        int yearsAgo,
        CancellationToken cancellation)
    {
        if (_settings == null)
            return new List<UnifiedCandleRecord>();

        try
        {
            var months = yearsAgo * 12;
            var url = _settings.CandleChartUrl
                .Replace("@GlobalLocationId", symbol)
                .Replace("@DurationMonths", months.ToString());

            var candleSeries = await KilidMonthlyData(symbol, url);

            return candleSeries;
        }
        catch
        {
            return new List<UnifiedCandleRecord>();
        }
    }

    private async Task<List<UnifiedCandleRecord>> KilidMonthlyData(string globalLocationId, string url)
    {
        try
        {
            var (data, jsonString) = await _fetcher.GetAsync(url);

            var candles = new List<UnifiedCandleRecord>();

            if (jsonString != null)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                KilidResponse response = JsonSerializer.Deserialize<KilidResponse>(jsonString, options);

                candles = response.Trends.Select(t => new UnifiedCandleRecord
                {
                    Date = t.Date,                                // تاریخ میلادی
                                                                  //PersianDate = t.PeriodSymbol,          // ماه فارسی مثل "دی - 1402"
                    Open = (decimal)t.Value,                              // مقدار قیمت
                    Close = (decimal)t.Value,
                    High = (decimal)t.Value,
                    Low = (decimal)t.Value,
                    //Timestamp = new DateTimeOffset(t.Date).ToUnixTimeSeconds(),
                    Volume = 0,
                }).ToList();
            }

            return candles;

        }
        catch (Exception ex)
        {
            //Log.Fatal(ex, $"There was a problem at KilidCandelClient - Url : {url}");
            return new List<UnifiedCandleRecord>();
        }
    }

    private class Trend
    {
        public DateTime Date { get; set; }
        public string PeriodSymbol { get; set; }
        public double Value { get; set; }
    }

    private class KilidResponse
    {
        public List<Trend> Trends { get; set; }
    }
}
