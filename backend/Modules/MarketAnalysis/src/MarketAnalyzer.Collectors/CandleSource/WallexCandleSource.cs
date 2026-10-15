using MarketAnalyzer.Collector;
using MarketAnalyzer.Collectors;
using MarketAnalyzer.Domain;
using MarketAnalyzer.Analysis.Infrastructure;
using Microsoft.Extensions.Options;

public class WallexCandleSource : ICandleSource
{
    private readonly CandleSettings? _settings;
    private readonly IDataFetcher _fetcher;

    public MarketType MarketType => MarketType.CryptoToman;
    public SourceType SourceType => SourceType.Wallex;    

    public WallexCandleSource(
        IOptions<UrlSettings> settings,
        IDataFetcher fetcher)
    {
        _settings = settings.Value.WallexSettings;
        _fetcher = fetcher;

        if (_settings != null && string.IsNullOrWhiteSpace(_settings.CandleChartUrl))
            _settings = null;
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

            var currencyCode = "تومان";
            var encodedCurrencyCode = Uri.EscapeDataString(currencyCode);

            var url = _settings.CandleChartUrl
                .Replace("@Symbol", symbol)
                .Replace("@From", from)
                .Replace("@To", to)
                .Replace("@Resolution", resolution)
                .Replace("@CurrencyCode", encodedCurrencyCode);

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

                        Open = TomanToRial(Convert.ToDecimal(data["o"][i])),
                        High = TomanToRial(Convert.ToDecimal(data["h"][i])),
                        Low = TomanToRial(Convert.ToDecimal(data["l"][i])),
                        Close = TomanToRial(Convert.ToDecimal(data["c"][i])),
                    });
                }
            }

            return model
                .OrderBy(x => x.Date)
                .ToList();
        }
        catch
        {
            return new List<UnifiedCandleRecord>();
        }
    }

    private decimal TomanToRial(decimal value) => value * 10;
}
