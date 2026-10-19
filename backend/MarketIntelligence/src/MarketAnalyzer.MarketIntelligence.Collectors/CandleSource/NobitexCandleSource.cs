using MarketAnalyzer.Collector;
using MarketAnalyzer.MarketIntelligence.Collectors;
using MarketAnalyzer.MarketIntelligence.Domain;
using MarketAnalyzer.MarketIntelligence.Infrastructure;
using Microsoft.Extensions.Options;

public class NobitexCandleSource : ICandleSource
{
    private readonly CandleSettings? _settings;
    private readonly IDataFetcher _fetcher;

    public MarketType MarketType => MarketType.CryptoToman;
    public SourceType SourceType => SourceType.Nobitex;

    public NobitexCandleSource(
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

            string currencyCode = "تومان";
            // کد کردن متن فارسی
            string encodedCurrencyCode = Uri.EscapeDataString(currencyCode);

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
                    var op = data["o"][i];
                    var tp = data["t"][i];
                    var hp = Convert.ToDecimal(data["h"][i]);
                    var lp = data["l"][i];
                    model.Add(new UnifiedCandleRecord()
                    {
                        //Timestamp = data["t"][i],
                        Date = DateTimeExtensions.UnixTimeStampToDateTime(Convert.ToInt64(data["t"][i])),
                        Open = TomanToRial(Convert.ToDecimal(data["o"][i])),
                        High = TomanToRial(Convert.ToDecimal(data["h"][i])),
                        Low = TomanToRial(Convert.ToDecimal(data["l"][i])),
                        Close = TomanToRial(Convert.ToDecimal(data["c"][i])),
                        //Volume = Convert.ToDecimal((string)data["v"][i])
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
