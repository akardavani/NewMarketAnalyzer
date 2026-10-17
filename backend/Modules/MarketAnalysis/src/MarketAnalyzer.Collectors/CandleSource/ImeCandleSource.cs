using MarketAnalyzer.Collector;
using MarketAnalyzer.Collectors;
using MarketAnalyzer.Domain;
using MarketAnalyzer.Analysis.Infrastructure;
using Microsoft.Extensions.Options;
using System.Text.Json;

public class ImeCandleSource : ICandleSource, IBaseImeFutureSymbolsProvider
{
    private readonly UrlSettings _settings;
    private readonly IDataFetcher _fetcher;
    private readonly IPostDataFetcher _postDataFetcher;
    private string _contractCode = "";
    private string _contractDescription = "";
    private string _deliveryDate = "";

    public MarketType MarketType => MarketType.Ime;
    public SourceType SourceType => SourceType.Ime;

    public ImeCandleSource(
        IOptions<UrlSettings> settings,
        IDataFetcher fetcher,
        IPostDataFetcher postDataFetcher)
    {
        _settings = settings.Value;

        if (_settings?.ImeSettings?.CandleChartUrl == null)
            throw new ArgumentNullException("ImeUrl is null in Settings");

        _fetcher = fetcher;
        _postDataFetcher = postDataFetcher;
    }

    public async Task<List<UnifiedCandleRecord>> FetchCandlesAsync(
        string symbol,
        Timeframe timeframe,
        int yearsAgo,
        CancellationToken cancellation)
    {
        var models = await FetchModelsAsync(
            symbol: symbol,
            timeframe: timeframe,
            yearsAgo: yearsAgo,
            cancellationToken: cancellation);

        return DataToCandle(models);
    }

    public async Task<ImeSymbol> GetSymbolsAsync(
        string symbol,
        Timeframe timeframe,
        int yearsAgo,
        CancellationToken cancellation)
    {
        var models = await FetchModelsAsync(
            symbol: symbol,
            timeframe: timeframe,
            yearsAgo: yearsAgo,
            cancellationToken: cancellation);

        return DataToSymbol(models);
    }

    private List<UnifiedCandleRecord> DataToCandle(List<Model> model)
    {
        return model.Select(e => new UnifiedCandleRecord
        {
            MarketId = e.MarketId,
            SymbolKey = e.SymbolKey,
            TimeframeId = e.TimeframeId,

            Date = e.Timestamp,
            PersianDate = e.PersianDate,
            Open = e.Open,
            High = e.High,
            Low = e.Low,
            Close = e.Close,
            Volume = e.Volume,
        }).ToList();
    }

    private async Task<List<Model>> FetchModelsAsync(
        string symbol,
        Timeframe timeframe,
        int yearsAgo,
        CancellationToken cancellationToken)
    {
        try
        {
            var from = DateTimeExtensions
                .ToPersian(DateTime.Now.AddYears(-yearsAgo))
                .Replace("-", "/");

            var to = DateTimeExtensions
                .ToPersian(DateTime.Now)
                .Replace("-", "/");

            var url = _settings.ImeSettings.CandleChartUrl
                .Replace("@Symbol", symbol)
                .Replace("@From", from)
                .Replace("@To", to);

            var getData = await _fetcher.GetAsync(url);

            return DataToModel(
                data: getData.text,
                symbol: symbol,
                timeframe: timeframe);
        }
        catch
        {
            return new List<Model>();
        }
    }


    //private async Task<List<UnifiedCandleRecord>> GetByUrl(
    //    string url,
    //    string symbol,
    //    Timeframe timeframe)
    //{
    //    try
    //    {
    //        var getData = await _fetcher.GetAsync(url);
    //        var models = DataToModel(
    //            data: getData.text,
    //            symbol: symbol,
    //            timeframe: timeframe);

    //        await InsertSymbols(models);

    //        return DataToCancle(models);
    //    }
    //    catch
    //    {
    //        return new List<UnifiedCandleRecord>();
    //    }
    //}

    private ImeSymbol DataToSymbol(List<Model> data)
    {
        var ss = data.Select(e => new ImeSymbol
        {
            Symbol = e.SymbolKey,
            DeliveryDate = e.DeliveryDate,
            PersianDeliveryDate = e.PersianDeliveryDate,
            PersianName = e.PersianName,
            Id = Convert.ToInt64(e.InsCode),
        })
        .FirstOrDefault();
        return ss;
    }

    //public async Task<List<UnifiedCandleRecord>> GetBaseImeFutureCandlesAsync(
    //    string symbol,
    //    Timeframe timeframe)
    //{
    //    var url = _settings.BaseImeSettings.CandleChartUrl;

    //    var payload = new CDCTradesPayload
    //    {
    //        FromDate = DateTime.UtcNow.AddDays(-50).ToString("yyyy-MM-dd"),
    //        ToDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
    //        PageNumber = 1,
    //        PageSize = 200,
    //        MarketId = null,
    //        MainGroupId = null,
    //        GroupId = null,
    //        SubGroupId = null,
    //        ProducerId = null,
    //        SupplierId = null,
    //        MarketIds = null,
    //        MainGroupIds = null,
    //        GroupIds = null,
    //        SubGroupIds = null,
    //        ProducerIds = null,
    //        SupplierIds = null,
    //        CommodityId = null,
    //        CommodityIds = null,
    //        Commodity = null,
    //        CustomFilter = "21",
    //        SortField = null,
    //        SortOrder = "asc",
    //        CIsinPrefix = null,
    //        ValidStatus = null
    //    };

    //    var jsonContent = JsonSerializer.Serialize(
    //        payload,
    //        new JsonSerializerOptions { WriteIndented = true });

    //    var request = new PostDataFetcherRequest
    //    {
    //        Url = url,
    //        Symbol = symbol,
    //        Content = jsonContent
    //    };

    //    var data = await _postDataFetcher.PostAsync(request);

    //    return DataToModel(
    //        data: data.text,
    //        symbol: symbol,
    //        timeframe: timeframe);
    //}

    private List<Model> DataToModel(
        string data,
        string symbol,
        Timeframe timeframe)
    {
        var root = JsonDocument.Parse(data).RootElement;

        var model = new List<Model>();

        static bool TryGetDataArray(JsonElement root, out JsonElement data)
        {
            if (root.TryGetProperty("Data", out data)) return true;
            if (root.TryGetProperty("rows", out data)) return true;

            data = default;
            return false;
        }

        if (TryGetDataArray(root, out var dataArray))
        {
            foreach (var r in dataArray.EnumerateArray())
            {
                decimal open = 0;
                decimal high = 0;
                decimal low = 0;
                decimal close = 0;
                decimal volume = 0;
                decimal netValue = 0;

                long timestamp = 0;
                DateTime date = DateTime.MinValue;

                if (r.TryGetProperty("ContractCode", out var pContractCode))
                    _contractCode = pContractCode.GetString() ?? "";

                if (r.TryGetProperty("ContractDescription", out var pContractDescription))
                    _contractDescription = pContractDescription.GetString() ?? "";

                if (r.TryGetProperty("DeliveryDate", out var pDeliveryDate))
                    _deliveryDate = pDeliveryDate.GetString() ?? "";

                if (r.TryGetProperty("FirstPrice", out var pOpen))
                    open = (decimal)pOpen.GetDouble();

                if (r.TryGetProperty("MaxPrice", out var pHigh))
                    high = (decimal)pHigh.GetDouble();

                if (r.TryGetProperty("MinPrice", out var pLow))
                    low = (decimal)pLow.GetDouble();

                if (r.TryGetProperty("LastPrice", out var pClose))
                    close = (decimal)pClose.GetDouble();

                if (r.TryGetProperty("TradesVolume", out var pVol))
                    volume = (decimal)pVol.GetDouble();

                if (r.TryGetProperty("TradesValue", out var pValue))
                    netValue = (decimal)pValue.GetDouble();

                if (r.TryGetProperty("DT", out var pDT))
                {
                    var dtStr = pDT.GetString();

                    if (!string.IsNullOrWhiteSpace(dtStr))
                    {
                        var parts = dtStr.Split('/');

                        if (parts.Length == 3)
                        {
                            date = DateTimeExtensions
                                .ShamsiToMiladi(dtStr)
                                .ToDateTime(TimeOnly.MinValue);

                            timestamp = new DateTimeOffset(date).ToUnixTimeSeconds();
                        }
                        else
                        {
                            date = DateTime.Parse(dtStr);
                            timestamp = new DateTimeOffset(date).ToUnixTimeSeconds();
                        }
                    }
                }

                if (close == 0)
                    continue;

                model.Add(new Model
                {
                    MarketId = MarketType,
                    SymbolKey = _contractCode,
                    TimeframeId = timeframe,

                    PersianName = _contractDescription,
                    DeliveryDate = _deliveryDate.ToGregorian(),
                    PersianDeliveryDate = _deliveryDate,

                    Timestamp = date,
                    PersianDate = DateTimeExtensions.ToPersian(date),
                    InsCode = symbol,
                    Open = open,
                    High = high,
                    Low = low,
                    Close = close,
                    Volume = volume,
                    //NetValue = netValue
                });
            }
        }

        return model
            .OrderBy(x => x.Timestamp)
            .ToList();
    }

    

    public async Task<List<CandleModel>> GetBaseImeFutureCandlesAsync(string symbol)
    {
        var url = _settings.BaseImeSettings.CandleChartUrl;
        var payload = new CDCTradesPayload
        {
            FromDate = DateTime.UtcNow.AddDays(-50).ToString("yyyy-MM-dd"),
            ToDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            PageNumber = 1,
            PageSize = 200,
            MarketId = null,
            MainGroupId = null,
            GroupId = null,
            SubGroupId = null,
            ProducerId = null,
            SupplierId = null,
            MarketIds = null,
            MainGroupIds = null,
            GroupIds = null,
            SubGroupIds = null,
            ProducerIds = null,
            SupplierIds = null,
            CommodityId = null,
            CommodityIds = null,
            Commodity = null,
            CustomFilter = "21",
            SortField = null,
            SortOrder = "asc",
            CIsinPrefix = null,
            ValidStatus = null
        };

        var jsonContent = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
        var request = new PostDataFetcherRequest()
        {
            Url = url,
            Symbol = symbol,
            Content = jsonContent
        };
        var data = await _postDataFetcher.PostAsync(request);

        var model = DataToModel(data.text);
        return model;
    }

    private List<CandleModel> DataToModel(string data)
    {
        var root = JsonDocument.Parse(data).RootElement;
        string pretty = JsonSerializer.Serialize(root, new JsonSerializerOptions { WriteIndented = true });

        var model = new List<CandleModel>();

        static bool TryGetDataArray(JsonElement root, out JsonElement data)
        {
            if (root.TryGetProperty("Data", out data)) return true;
            if (root.TryGetProperty("rows", out data)) return true;
            data = default;
            return false;
        }

        if (TryGetDataArray(root, out var dataArray))
        {
            foreach (var r in dataArray.EnumerateArray())
            {
                decimal open = 0, high = 0, low = 0, close = 0, volume = 0, netValue = 0;
                long ts = 0;
                DateTime date = DateTime.MinValue;

                if (r.TryGetProperty("ContractCode", out var pContractCode)) _contractCode = pContractCode.GetString();
                if (r.TryGetProperty("ContractDescription", out var pContractDescription)) _contractDescription = pContractDescription.GetString();
                if (r.TryGetProperty("DeliveryDate", out var pDeliveryDate)) _deliveryDate = pDeliveryDate.GetString();

                if (r.TryGetProperty("FirstPrice", out var pOpen)) open = (decimal)pOpen.GetDouble();
                if (r.TryGetProperty("MaxPrice", out var pHigh)) high = (decimal)pHigh.GetDouble();
                if (r.TryGetProperty("MinPrice", out var pLow)) low = (decimal)pLow.GetDouble();
                if (r.TryGetProperty("LastPrice", out var pClose)) close = (decimal)pClose.GetDouble();
                if (r.TryGetProperty("TradesVolume", out var pVol)) volume = (decimal)pVol.GetDouble();
                if (r.TryGetProperty("TradesValue", out var pValue)) netValue = (decimal)pValue.GetDouble();

                if (r.TryGetProperty("DT", out var pDT))
                {
                    var dtStr = pDT.GetString();
                    if (!string.IsNullOrEmpty(dtStr))
                    {
                        var parts = dtStr.Split('/');
                        if (parts.Length == 3)
                        {
                            int y = int.Parse(parts[0]);
                            int m = int.Parse(parts[1]);
                            int d = int.Parse(parts[2]);
                            date = DateTimeExtensions.ShamsiToMiladi(dtStr).ToDateTime(TimeOnly.MinValue);
                            ts = new DateTimeOffset(date).ToUnixTimeSeconds();
                        }
                        else
                        {
                            date = DateTime.Parse(dtStr);
                            ts = new DateTimeOffset(date).ToUnixTimeSeconds();
                        }
                    }
                }

                if (close == 0) continue;

                model.Add(new CandleModel
                {
                    Date = date,
                    Open = open,
                    High = high,
                    Low = low,
                    Close = close,
                    Volume = volume,
                    UnixTimestamp = ts,
                    Timestamp = ts,
                    NetValue = netValue
                });
            }
        }

        return model.OrderBy(x => x.Timestamp).ToList();
    }

    private class CDCTradesPayload
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string MarketId { get; set; }
        public string MainGroupId { get; set; }
        public string GroupId { get; set; }
        public string SubGroupId { get; set; }
        public string ProducerId { get; set; }
        public string SupplierId { get; set; }
        public string MarketIds { get; set; }
        public string MainGroupIds { get; set; }
        public string GroupIds { get; set; }
        public string SubGroupIds { get; set; }
        public string ProducerIds { get; set; }
        public string SupplierIds { get; set; }
        public string CommodityId { get; set; }
        public string CommodityIds { get; set; }
        public string Commodity { get; set; }
        public string CustomFilter { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }
        public string CIsinPrefix { get; set; }
        public string ValidStatus { get; set; }
    }

    private class Model
    {
        public long Id { get; set; }
        // برای اینکه همه بازارها یک جدول داشته باشند
        public MarketType MarketId { get; set; }
        // زمان‌بندی
        public Timeframe TimeframeId { get; set; }
        // نماد
        public string SymbolKey { get; set; } = null!;
        public string InsCode { get; set; }
        public string PersianName { get; set; }

        // داده‌های کندل
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Value { get; set; }

        public DateTime Timestamp { get; set; }
        public DateOnly DeliveryDate { get; set; }
        public string PersianDeliveryDate { get; set; }
        public string PersianDate { get; set; } = "";
    }
}
