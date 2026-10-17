using MarketAnalyzer.Collector;
using MarketIntelligence.Collectors;
using MarketIntelligence.Domain;
using MarketIntelligence.Infrastructure;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;

public class KucoinCandleSource : ICandleSource, ICryptoSpotSymbolsProvider
{
    private readonly UrlSettings? _settings;
    private readonly IDataFetcher _fetcher;

    public MarketType MarketType => MarketType.Crypto;
    public SourceType SourceType => SourceType.Kucoin;

    public KucoinCandleSource(
        IOptions<UrlSettings> settings,
        IDataFetcher fetcher)
    {
        _settings = settings.Value;
        _fetcher = fetcher;

        if (_settings?.KucoinSettings == null ||
            string.IsNullOrWhiteSpace(_settings.KucoinSettings.CandleChartUrl))
        {
            _settings = null;
        }
    }

    public async Task<List<CryptoSymbol>> GetSpotSymbolsAsync()
    {
        if (_settings == null)
            return new List<CryptoSymbol>();

        try
        {
            var (data, _) = await _fetcher.GetAsync(_settings.KucoinSettings.SymbolUrl);

            if (data == null || data["data"] == null)
                return new List<CryptoSymbol>();

            var jsonArray = data["data"] as JArray;

            if (jsonArray == null)
                return new List<CryptoSymbol>();

            var symbols = jsonArray.ToObject<List<BaseKucoinCryptoSymbol>>();

            return symbols?
                .Select(s => new CryptoSymbol(s))
                .ToList() ?? new List<CryptoSymbol>();
        }
        catch
        {
            return new List<CryptoSymbol>();
        }
    }

    public string GetKucoinFromTimeFrameToUrl(Timeframe timeFrame)
    {
        return timeFrame switch
        {
            Timeframe.FiveMinutes => "5min",
            Timeframe.FifteenMinutes => "15min",
            Timeframe.OneHour => "1hour",
            Timeframe.FourHours => "4hour",
            Timeframe.Daily => "1day",
            Timeframe.Weekly => "1week",
            _ => "1day",
        };
    }

    public string GetKucoinBeginToEndFromTimeFrameToUrl(Timeframe timeFrame)
    {
        var end = DateTime.UtcNow.AddDays(5).DateTimeToUnixTimestamp();

        return timeFrame switch
        {
            Timeframe.FiveMinutes => $"begin={DateTime.UtcNow.AddDays(-2).DateTimeToUnixTimestamp()}&end={end}",
            Timeframe.FifteenMinutes => $"begin={DateTime.UtcNow.AddDays(-6).DateTimeToUnixTimestamp()}&end={end}",
            Timeframe.OneHour => $"begin={DateTime.UtcNow.AddDays(-20).DateTimeToUnixTimestamp()}&end={end}",
            Timeframe.FourHours => $"begin={DateTime.UtcNow.AddDays(-85).DateTimeToUnixTimestamp()}&end={end}",
            Timeframe.Daily => $"begin={DateTime.UtcNow.AddDays(-500).DateTimeToUnixTimestamp()}&end={end}",
            Timeframe.Weekly => $"begin={DateTime.UtcNow.AddMonths(-110).DateTimeToUnixTimestamp()}&end={end}",
            _ => $"begin={DateTime.UtcNow.AddDays(-500).DateTimeToUnixTimestamp()}&end={end}",
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
            var urlTemplate = _settings.KucoinSettings.CandleChartUrl;

            var timeFrame = GetKucoinFromTimeFrameToUrl(timeframe);
            var beginToEnd = GetKucoinBeginToEndFromTimeFrameToUrl(timeframe);

            var url = urlTemplate
                .Replace("@Symbol", symbol)
                .Replace("@TimeFrame", timeFrame)
                .Replace("@BeginToEnd", beginToEnd);

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
            var (data, _) = await _fetcher.GetAsync(url);

            if (data?["data"] == null)
                return new List<UnifiedCandleRecord>();

            var list = new List<UnifiedCandleRecord>();

            foreach (var item in data["data"])
            {
                try
                {
                    var dic = CreateDic(item);

                    var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var timestamp = dateTime.AddSeconds(Convert.ToInt64(dic[0])).ToLocalTime();
                    list.Add(new UnifiedCandleRecord
                    {
                        MarketId = MarketType,
                        SymbolKey = symbol,
                        TimeframeId = timeframe,

                        Date = timestamp,
                        PersianDate = DateTimeExtensions.ToPersian(timestamp),

                        Open = Convert.ToDecimal(dic[1]),
                        Close = Convert.ToDecimal(dic[2]),
                        High = Convert.ToDecimal(dic[3]),
                        Low = Convert.ToDecimal(dic[4]),
                        Volume = Convert.ToDecimal(dic[5])
                    });
                }
                catch
                {
                    // optional log
                }
            }

            return list
                .OrderBy(x => x.Date)
                .ToList();
        }
        catch
        {
            return new List<UnifiedCandleRecord>();
        }
    }

    private static Dictionary<int, string> CreateDic(dynamic json)
    {
        var dic = new Dictionary<int, string>();
        var splitText = json.ToString().Split(",");

        for (int i = 0; i < splitText.Length; i++)
        {
            var val = splitText[i]
                .ToString()
                .Replace("[", "")
                .Replace("\"", "")
                .Replace(Environment.NewLine, "")
                .Trim();

            dic[i] = val;
        }

        return dic;
    }
}
