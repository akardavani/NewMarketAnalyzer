using MarketAnalyzer.Collector;
using MarketAnalyzer.Collectors;
using MarketAnalyzer.Domain;
using MarketAnalyzer.Analysis.Infrastructure;
using Microsoft.Extensions.Options;

public class TgjuCandleSource : ICandleSource
{
    private readonly UrlSettings _settings;
    private readonly IDataFetcher _fetcher;

    public MarketType MarketType => MarketType.Macro;
    public SourceType SourceType => SourceType.tgju;

    public TgjuCandleSource(
        IOptions<UrlSettings> settings,
        IDataFetcher fetcher)
    {
        _settings = settings.Value;
        if (_settings?.tgjuSettings?.CandleChartUrl == null)
            throw new ArgumentNullException("tgjuUrl is null in Settings");
        _fetcher = fetcher;
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
            var from = DateTime.Now.AddYears(-1 * yearsAgo).DateTimeToUnixTimestamp();
            var to = DateTime.Now.DateTimeToUnixTimestamp();
            var resolution = GetNahayatnegarFromTimeFrameToUrl(timeframe);

            var url = _settings.tgjuSettings.CandleChartUrl
                .Replace("@Symbol", symbol)
                .Replace("@From", from)
                .Replace("@To", to)
                .Replace("@Resolution", resolution);

            return await GetByUrl(url, symbol, timeframe);
        }
        catch
        {
            return new List<UnifiedCandleRecord>();
        }
    }

    public string GetNahayatnegarFromTimeFrameToUrl(Timeframe timeFrame)
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
}
