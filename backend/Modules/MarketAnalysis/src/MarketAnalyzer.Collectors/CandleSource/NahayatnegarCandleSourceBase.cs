using MarketAnalyzer.Collector;
using MarketAnalyzer.Collectors;
using MarketAnalyzer.Domain;
using MarketAnalyzer.Analysis.Infrastructure;
using Microsoft.Extensions.Options;

public abstract class NahayatnegarCandleSourceBase : ICandleSource
{
    private readonly CandleSettings? _settings;
    private readonly IDataFetcher _fetcher;

    public abstract MarketType MarketType { get; }
    public abstract SourceType SourceType { get; }

    protected NahayatnegarCandleSourceBase(
        IOptions<UrlSettings> settings,
        IDataFetcher fetcher)
    {
        _settings = settings.Value.NahayatnegarSettings;
        _fetcher = fetcher;

        if (_settings != null && string.IsNullOrWhiteSpace(_settings.CandleChartUrl))
            _settings = null;
    }

    private string GetNahayatnegarFromTimeFrameToUrl(Timeframe timeFrame)
    {
        return timeFrame switch
        {
            Timeframe.FiveMinutes => "5m",
            Timeframe.FifteenMinutes => "15m",
            Timeframe.OneHour => "1",
            Timeframe.FourHours => "4h",
            Timeframe.Daily => "1D",
            Timeframe.Weekly => "1W",
            Timeframe.Mountly => "1M",
            _ => "1D",
        };
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
            var type = GetTypeInstrument(symbol);
            var resolution = GetNahayatnegarFromTimeFrameToUrl(timeframe);
            var from = DateTime.Now.AddYears(-1 * yearsAgo).DateTimeToUnixTimestamp();
            var to = DateTime.Now.DateTimeToUnixTimestamp();
            var adjustmentType = "3";

            var url = _settings.CandleChartUrl
                .Replace("@Symbol", symbol + adjustmentType)
                .Replace("@Resolution", resolution)
                .Replace("@From", from)
                .Replace("@To", to)
                .Replace("@Type", type)
                .Replace("@AdjustmentType", adjustmentType);

            return await GetByUrl(url, symbol, timeframe);
        }
        catch
        {
            return new List<UnifiedCandleRecord>();
        }
    }

    private async Task<List<UnifiedCandleRecord>> GetByUrl(
        string url,
        string symbol,
        Timeframe timeframe)
    {
        try
        {
            var getData = await _fetcher.GetAsync(url);
            var data = getData.data;
            var model = new List<UnifiedCandleRecord>();

            if (data != null)
            {
                for (int i = 0; i < data["o"].Count; i++)
                {
                    var timestamp = DateTimeExtensions.UnixTimeStampToDateTime(Convert.ToInt64(data["t"][i]));

                    model.Add(new UnifiedCandleRecord
                    {
                        MarketId = MarketType,
                        SymbolKey = symbol,
                        TimeframeId = timeframe,

                        Date = timestamp,
                        PersianDate = DateTimeExtensions.ToPersian(timestamp),

                        Open = Convert.ToDecimal((string)data["o"][i]),
                        High = Convert.ToDecimal((string)data["h"][i]),
                        Low = Convert.ToDecimal((string)data["l"][i]),
                        Close = Convert.ToDecimal((string)data["c"][i]),
                        Volume = Convert.ToDecimal((string)data["v"][i])
                    });
                }
            }

            return model.OrderBy(x => x.Date).ToList();
        }
        catch
        {
            return new List<UnifiedCandleRecord>();
        }
    }

    private static string GetTypeInstrument(string symbol)
    {
        return symbol.StartsWith("IRR")
            ? "rights"
            : "stock";
    }
}

