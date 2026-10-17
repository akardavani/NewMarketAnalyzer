using MarketAnalyzer.Collector;
using MarketAnalyzer.ConnectedServices.TsePublic;
using MarketIntelligence.Domain;
using MarketIntelligence.Infrastructure;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using static MarketAnalyzer.ConnectedServices.TsePublic.TsePublicV2SoapClient;
using static System.Net.WebRequestMethods;

namespace MarketIntelligence.Collectors
{
    internal class TseTmcClient : ITseTmcClient
    {
        private readonly TseTmcWebServiceSetting _tsetmcServiceSettings;
        private readonly TseTmcSettings _tsetmcSettings;
        private readonly IDataFetcher _fetcher;
        List<byte> tseFlowMarkets = new List<byte>();

        public TseTmcClient(IOptions<UrlSettings> settings, IDataFetcher fetcher)
        {
            var _settings = settings.Value;


            if (_settings != null || settings?.Value.TseTmcWebServiceSettings != null)
            {
                _tsetmcServiceSettings = settings.Value.TseTmcWebServiceSettings;
            }

            if (_settings != null || settings?.Value.TseTmcSettings != null)
            {
                _tsetmcSettings = settings.Value.TseTmcSettings;
            }

            tseFlowMarkets.Add((int)TseFlowMarket.Bourse);
            tseFlowMarkets.Add((int)TseFlowMarket.FaraBourse);
            tseFlowMarkets.Add((int)TseFlowMarket.OTCBase);
            tseFlowMarkets.Add((int)TseFlowMarket.Mercantile);
            _fetcher = fetcher;
        }

        public async Task<List<TseTradeLastDay>> GetTradeLastDayAsync(CancellationToken cancellation = default)
        {
            var list = new List<TseTradeLastDay>();
            try
            {
                var client = new TsePublicV2SoapClient(EndpointConfiguration.TsePublicV2Soap12);
                var elements = new List<XElement>();

                foreach (var item in tseFlowMarkets)
                {
                    var tradeLastDay = await client.TradeLastDayAsync(_tsetmcServiceSettings.User, _tsetmcServiceSettings.Password, item);

                    if (tradeLastDay != null)
                    {
                        var tse = tradeLastDay.Nodes[1].Elements("TradeLastDay").Elements("TradeLastDay").ToList();
                        elements.AddRange(tse);
                    }
                }

                if (elements.Count > 0)
                {
                    list = elements
                        .Select(ins => new TseTradeLastDay()
                        {
                            InsCode = (long)ins.Element("InsCode"),
                            Date = DateTimeExtensions.StringToDateTime((string)ins.Element("DEven")),
                            PriceFirst = (decimal)ins.Element("PriceFirst"),
                            Closing = (decimal)ins.Element("PClosing"),
                            LastPrice = (decimal)ins.Element("PDrCotVal"),
                            Count = (long)ins.Element("ZTotTran"),
                            Volume = (long)ins.Element("QTotTran5J"),
                            TransactionValue = (decimal)ins.Element("QTotCap"),
                            PriceChange = (decimal)ins.Element("PriceChange"),
                            PriceMin = (decimal)ins.Element("PriceMin"),
                            PriceMax = (decimal)ins.Element("PriceMax"),
                            PriceYesterday = (decimal)ins.Element("PriceYesterday"),
                            Last = (int)ins.Element("Last"),
                            Time = (int)ins.Element("HEven")
                        }).ToList();

                    //Log.Information("TradeLastDay saved successfully");
                }
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "There was a problem at SaveTradeLastDayAsync");
            }


            return list;
        }

        public async Task<List<TseStaticThreshold>> GetStaticThresholdAsync(CancellationToken cancellation = default)
        {
            var list = new List<TseStaticThreshold>();
            try
            {
                var client = new TsePublicV2SoapClient(EndpointConfiguration.TsePublicV2Soap12);
                var elements = new List<XElement>();

                foreach (var item in tseFlowMarkets)
                {
                    var staticThresholds = await client.StaticThresholdsAsync(_tsetmcServiceSettings.User, _tsetmcServiceSettings.Password, item);

                    if (staticThresholds != null)
                    {
                        var tse = staticThresholds.Nodes[1].Elements("StaticThresholds").Elements("StaticThresholds").ToList();
                        elements.AddRange(tse);
                    }
                }

                if (elements.Count > 0)
                {
                    list = elements
                        .Select(ins => new TseStaticThreshold()
                        {
                            InsCode = (long)ins.Element("InsCode"),
                            MaxRange = (decimal)ins.Element("PSGelStaMax"),
                            MinRange = (decimal)ins.Element("PSGelStaMin"),
                            Date = DateTime.Today
                        }).ToList();

                    //Log.Information("StaticThreshold saved successfully");
                }
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "There was a problem at SaveStaticThresholdAsync");
            }

            return list;
        }

        public async Task<List<TseBestLimit>> GetBestLimitsAsync(CancellationToken cancellation = default)
        {
            var list = new List<TseBestLimit>();
            try
            {
                var client = new TsePublicV2SoapClient(EndpointConfiguration.TsePublicV2Soap12);
                var elements = new List<XElement>();

                foreach (var item in tseFlowMarkets)
                {
                    var bestLimits = await client.BestLimitsAllInsAsync(_tsetmcServiceSettings.User, _tsetmcServiceSettings.Password, item);

                    if (bestLimits != null)
                    {
                        var tse = bestLimits.Nodes[1].Elements("AllBestLimits").Elements("InstBestLimit").ToList();
                        elements.AddRange(tse);
                    }
                }

                if (elements.Count > 0)
                {
                    list = elements
                        .Select(ins => new TseBestLimit()
                        {
                            InsCode = (long)ins.Element("InsCode"),
                            Date = DateTime.Today,
                            Number = (int)ins.Element("number"),
                            BuyVolume = (long)ins.Element("QTitMeDem"),
                            BuyQuantity = (long)ins.Element("ZOrdMeDem"),
                            BuyRequestQuantity = (long)ins.Element("PMeDem"),
                            SellVolume = (long)ins.Element("QTitMeOf"),
                            SellQuantity = (long)ins.Element("PMeOf"),
                            SellRequestQuantity = (long)ins.Element("ZOrdMeOf")
                        }).ToList();

                    //Log.Information("TseInstrument saved successfully");
                }
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "There was a problem at SaveBestLimitsAsync");
            }

            return list;
        }

        public async Task<List<TseSymbol>> GetStockInstrumentAsync(CancellationToken cancellation = default)
        {
            var list = new List<TseSymbol>();
            try
            {
                var client = new TsePublicV2SoapClient(EndpointConfiguration.TsePublicV2Soap12);
                var elements = new List<XElement>();

                foreach (var item in tseFlowMarkets)
                {
                    var instruments = await client.InstrumentAsync(_tsetmcServiceSettings.User, _tsetmcServiceSettings.Password, item);

                    if (instruments != null)
                    {
                        var tse = instruments.Nodes[1].Elements("Instruments")
                            .Elements("TseInstruments").ToList();
                        elements.AddRange(tse);
                    }
                }

                if (elements.Count > 0)
                {
                    list = elements
                        .Select(ins => new TseSymbol()
                        {
                            Date = (long)ins.Element("DEVen"),
                            InsCode = (long)ins.Element("InsCode"),
                            InstrumentId = (string)ins.Element("InstrumentID"),
                            InstrumentMnemonic = (string)ins.Element("CValMne"),
                            EnglishName = (string)ins.Element("LVal18"),
                            FourDigitCompanyCode = (string)ins.Element("CSocCSAC"),
                            CompanyName = ReplaceData((string)ins.Element("LSoc30")),
                            Symbol = ReplaceData((string)ins.Element("LVal18AFC")),
                            Name = ReplaceData((string)ins.Element("LVal30")),
                            CIsin = (string)ins.Element("CIsin"),
                            InstrumentGroupId = (string)ins.Element("CGrValCot"),
                            MarketSegment = (string)ins.Element("YMarNSC"),
                            BoardCode = (long)ins.Element("CComVal"),
                            SectorCode = ((string)ins.Element("CSecVal")).Trim(),
                            SubSectorCode = ((string)ins.Element("CSoSecVal")).Trim(),
                            InstrumentType = (long)ins.Element("YVal"),
                            SharesOrBondsNumber = (long)ins.Element("ZTitad"),
                            Flow = (int)ins.Element("Flow"),
                            Valid = (int)ins.Element("Valid")
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "There was a problem at SaveTseInstrumentAsync");
            }

            return list;
        }

        /// <summary>
        /// ایندکس ها رو روزانه میتونیم بگیریم
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<List<TseIndexSymbol>> GetAllIndexAsync(CancellationToken cancellation = default)
        {
            var list = new List<TseIndexSymbol>();
            try
            {
                var client = new TsePublicV2SoapClient(EndpointConfiguration.TsePublicV2Soap12);
                var elements = new List<XElement>();

                foreach (var item in tseFlowMarkets)
                {

                    var data = await client.IndexB2Async(_tsetmcServiceSettings.User, _tsetmcServiceSettings.Password, 20260608);

                    if (data != null)
                    {
                        var tse = data.Nodes[1]?
                            .Element("TseIndexB2")?
                            .Elements("TseIndexB2")?
                            .ToList();

                        if (tse?.Count > 0)
                        {
                            elements.AddRange(tse);
                        }
                    }
                }
                if (elements.Any())
                {
                    list = elements.Select(ins => new TseIndexSymbol
                    {
                        InsCode = ((long)ins.Element("InsCode")).ToString()
                    }).ToList();
                }

                //    //Log.Information("TradeLastDay saved successfully");
                //}
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "There was a problem at SaveTradeLastDayAsync");
            }

            foreach (var item in list)
            {
                await GetIndexByInsCode(item);
            }

            return list;
        }

        public async Task GetIndexByInsCode(TseIndexSymbol index)
        {
            try
            {
                var getData = await _fetcher.GetAsync(_tsetmcSettings.TseTmcIndexSymbol.Replace("@InsCode", index.InsCode));
                var text = getData.text;

                if (getData.data is not null)
                {
                    using var doc = JsonDocument.Parse(text);

                    var lVal30 = doc.RootElement
                        .GetProperty("instrumentInfo")
                        .GetProperty("lVal30")
                        .GetString();

                    var parts = lVal30.Split('-', 2);

                    if (parts is not null)
                    {
                        var sectorCode = parts[0];
                        var sectorName = parts[1];

                        index.SectorCode = sectorCode;
                        index.SectorName = sectorName.NormalizeArabicName();
                        index.Flow = 1; // پیشفرض بازار بورس
                    }
                }
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "There was a problem at GetETFByInsCode");
            }
        }

        public async Task<CandleSeries> GetIndexCandleByInsCode(string insCode)
        {
            var candles = new List<CandleModel>();
            try
            {
                var getData = await _fetcher.GetAsync(_tsetmcSettings.TseTmcIndexCandle.Replace("@InsCode", insCode));
                var text = getData.text;


                using var doc = JsonDocument.Parse(text);

                //var candles = new List<CandleModel>();

                var indexB2 = doc.RootElement.GetProperty("indexB2");

                foreach (var item in indexB2.EnumerateArray())
                {
                    var dateInt = item.GetProperty("dEven").GetInt32();
                    var date = DateTime.ParseExact(
                        dateInt.ToString(),
                        "yyyyMMdd",
                        CultureInfo.InvariantCulture
                    );

                    var open = item.GetProperty("xNivInuPbMresIbs").GetDecimal();
                    var close = item.GetProperty("xNivInuClMresIbs").GetDecimal();
                    var high = item.GetProperty("xNivInuPhMresIbs").GetDecimal();

                    var candle = new CandleModel
                    {
                        Date = date,
                        Open = open,
                        Close = close,
                        High = high,

                        // چون فعلاً در JSON نداریم
                        Low = 0,
                        Volume = 0,

                        Timestamp = new DateTimeOffset(date).ToUnixTimeMilliseconds(),
                        UnixTimestamp = new DateTimeOffset(date).ToUnixTimeSeconds(),
                        NetValue = 0
                    };

                    candles.Add(candle);
                }

                return new CandleSeries
                {
                    Candels = candles,
                    Symbol = insCode,
                    Timeframe = Timeframe.Daily,
                    marketType = (int)MarketType.TseIndex
                };
                //var list = new List<CandleModel>();
                //if (getData.data is not null)
                //{
                //    using var doc = JsonDocument.Parse(text);

                //    var lVal30 = doc.RootElement
                //        .GetProperty("indexB2")
                //        .GetString();

                //candles = text.Select(t => new CandleModel
                //{
                //    Date = DateTimeExtensions.StringToDateTime((string)text.Element("DEven")),                                // تاریخ میلادی
                //    //PersianDate = t.PeriodSymbol,          // ماه فارسی مثل "دی - 1402"
                //    Open = (decimal)t.Value,                              // مقدار قیمت
                //    Close = (decimal)t.Value,
                //    High = (decimal)t.Value,
                //    Low = (decimal)t.Value,
                //    Timestamp = new DateTimeOffset(t.Date).ToUnixTimeSeconds(),
                //    Volume = 0,
                //}).ToList();
                //var lVal30 = doc.RootElement
                //    .GetProperty("indexB2")
                //    .GetString();

                //var parts = lVal30.Split('-', 2);

                //if (parts is not null)
                //{
                //    var sectorCode = parts[0];
                //    var sectorName = parts[1];

                //    index.SectorCode = sectorCode;
                //    index.SectorName = sectorName.NormalizeArabicName();
                //    index.Flow = 1; // پیشفرض بازار بورس
                //}

                //list = elements.Select(ins => new TseIndexB2
                //{
                //    InsCode = (long)ins.Element("InsCode"),
                //    Date = DateTimeExtensions.StringToDateTime((string)ins.Element("DEven")),
                //    Time = (int)ins.Element("HEven"),
                //    XNivInuClMresIbs = (decimal)ins.Element("XNivInuClMresIbs"),
                //    HNivInuClMresIbs = (int)ins.Element("HNivInuClMresIbs"),
                //    XNivInuPrDifMresIbs = (decimal)ins.Element("XNivInuPrDifMresIbs"),
                //    HNivInuPrDifMresIbs = (int)ins.Element("HNivInuPrDifMresIbs"),
                //    XNivInuPhMresIbs = (decimal)ins.Element("XNivInuPhMresIbs"),
                //    HNivInuPhMresIbs = (int)ins.Element("HNivInuPhMresIbs"),
                //    XVarIdxPhJClV = (decimal)ins.Element("XVarIdxPhJClV"),
                //    XNivInuPbMresIbs = (decimal)ins.Element("XNivInuPbMresIbs"),
                //    HNivInuPbMresIbs = (int)ins.Element("HNivInuPbMresIbs"),
                //    XVarIdxPbJClV = (decimal)ins.Element("XVarIdxPbJClV"),
                //    XVarDrInuClV = (decimal)ins.Element("XVarDrInuClV"),
                //    QDvdNetJValIbs = (decimal)ins.Element("QDvdNetJValIbs"),
                //    QCapBsRfVIbs = (decimal)ins.Element("QCapBsRfVIbs"),
                //    KAjCapBzIbs = (decimal)ins.Element("KAjCapBzIbs"),
                //    XNivIrteNetClIbs = (decimal)ins.Element("XNivIrteNetClIbs")
                //}).ToList();
                //}

                //return new List<CandleSeries>();
            }
            catch (Exception ex)
            {
                //var ss= new CandleSeries { Candels = list, Symbol = insCode, Timeframe = TimeframeDto.Daily, marketType = MarketTypeDto.TseTmcIndex };
                return new CandleSeries();// { ss };
            }
        }

        private string ReplaceData(string data)
        {
            return data.NormalizeArabicName();
        }

        public async Task<TseTmcETFNavInfo> GetETFByInsCode(string insCode)
        {
            var info = new TseTmcETFNavInfo();
            try
            {
                var getData = await _fetcher.GetAsync(_tsetmcSettings.TseTmcETF.Replace("@InsCode", insCode));
                var text = getData.text;

                if (getData.data is not null)
                {
                    var model = JsonConvert.DeserializeObject<Etf>(text);
                    info = model.TseTmcETFNavInfo;
                }

                return info;
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "There was a problem at GetETFByInsCode");
                return info;
            }
        }

        public async Task<List<TseClientTypeInfo>> GetClientTypeAsync(string insCode)
        {
            var info = new TseClientTypeResponse();
            try
            {
                var url = $"https://cdn.tsetmc.com/api/ClientType/GetClientTypeHistory/{insCode}";
                info = await _fetcher.GetAsync<TseClientTypeResponse>(url);
                
                return info.ClientType;
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "There was a problem at GetETFByInsCode");
                return info.ClientType;
            }
        }

        public async Task<TseTmcETFNavInfo> GetMofidEasyFilterData()
        {
            var info = new TseTmcETFNavInfo();
            try
            {
                var getData = await _fetcher.GetAsync(_tsetmcSettings.MofidEasyFilterData);
                var text = getData.text;

                if (getData.data is not null)
                {
                    var model = JsonConvert.DeserializeObject<Etf>(text);
                    info = model.TseTmcETFNavInfo;
                }

                return info;
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "There was a problem at GetETFByInsCode");
                return info;
            }
        }

        // ************************************
        // ************************************        

        public async Task<List<TseIndexB1LastDayLastData>> GetIndexB1LastDayLastDataAsync(CancellationToken cancellation = default)
        {
            var list = new List<TseIndexB1LastDayLastData>();
            try
            {
                var client = new TsePublicV2SoapClient(EndpointConfiguration.TsePublicV2Soap12);
                var elements = new List<XElement>();

                foreach (var item in tseFlowMarkets)
                {
                    // اطلاعات شاخص ها 
                    // دارویی، خودرو، شاخص کل
                    var data = await client.IndexB1LastDayLastDataAsync(_tsetmcServiceSettings.User, _tsetmcServiceSettings.Password, item);

                    if (data != null)
                    {
                        var tse = data.Nodes[1]?
                            .Element("TseIndexB1LastDayLastData")?
                            .Elements("IndexB1LastDayLastData")?
                            .ToList();

                        if (tse?.Count > 0)
                        {
                            elements.AddRange(tse);
                        }
                    }
                }

                if (elements.Any())
                {
                    list = elements.Select(ins => new TseIndexB1LastDayLastData
                    {
                        LVal30 = (string)ins.Element("LVal30"),
                        InsCode = (long)ins.Element("InsCode"),
                        Date = DateTimeExtensions.StringToDateTime((string)ins.Element("DEven")),
                        Time = (int)ins.Element("HEven"),
                        ZValIdxCot = (int)ins.Element("ZValIdxCot"),
                        ZValBaiIbs = (int)ins.Element("ZValBaiIbs"),
                        ZValHauIbs = (int)ins.Element("ZValHauIbs"),
                        ZValIchgIbs = (int)ins.Element("ZValIchgIbs"),
                        ZValNonCotIbs = (int)ins.Element("ZValNonCotIbs"),
                        ZValResIbs = (int)ins.Element("ZValResIbs"),
                        ZValSuIbs = (int)ins.Element("ZValSuIbs"),
                        ZTotValIbs = (int)ins.Element("ZTotValIbs"),
                        HPhJIdx = (decimal)ins.Element("HPhJIdx"),
                        HPbJIdx = (decimal)ins.Element("HPbJIdx"),
                        XDrNivJIdx004 = (decimal)ins.Element("XDrNivJIdx004"),
                        XPhNivJIdx004 = (decimal)ins.Element("XPhNivJIdx004"),
                        XPbNivJIdx004 = (decimal)ins.Element("XPbNivJIdx004"),
                        XNivIrteNetIbs = (decimal)ins.Element("XNivIrteNetIbs"),
                        XVarIdxJ = (decimal)ins.Element("XVarIdxJ"),
                        XVarIdxJRfV = (decimal)ins.Element("XVarIdxJRfV"),
                        XMoyVarValIbs = (decimal)ins.Element("XMoyVarValIbs"),
                        XMoyVarValBaiIbs = (decimal)ins.Element("XMoyVarValBaiIbs"),
                        XmoyVarValHauIbs = (decimal)ins.Element("XmoyVarValHauIbs")
                    }).ToList();
                }

                //    //Log.Information("TradeLastDay saved successfully");
                //}
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "There was a problem at SaveTradeLastDayAsync");
            }


            return list;
        }

        public async Task<List<TseIndexB2>> GetIndexB1LastDayOneInstAsync(CancellationToken cancellation = default)
        {
            var list = new List<TseIndexB2>();
            try
            {
                var client = new TsePublicV2SoapClient(EndpointConfiguration.TsePublicV2Soap12);
                var elements = new List<XElement>();

                foreach (var item in tseFlowMarkets)
                {

                    var data = await client.IndexB1LastDayOneInstAsync(_tsetmcServiceSettings.User, _tsetmcServiceSettings.Password, 33878047680249697, 1);

                    if (data != null)
                    {
                        var tse = data.Nodes[1]?
                            .Element("TseIndexB1LastDay")?
                            .Elements("TseIndexB1LastDay")?
                            .ToList();

                        if (tse?.Count > 0)
                        {
                            elements.AddRange(tse);
                        }
                    }
                }
                if (elements.Any())
                {
                    list = elements.Select(ins => new TseIndexB2
                    {
                        InsCode = (long)ins.Element("InsCode"),
                        Date = DateTimeExtensions.StringToDateTime((string)ins.Element("DEven")),
                        Time = (int)ins.Element("HEven"),
                        XNivInuClMresIbs = (decimal)ins.Element("XNivInuClMresIbs"),
                        HNivInuClMresIbs = (int)ins.Element("HNivInuClMresIbs"),
                        XNivInuPrDifMresIbs = (decimal)ins.Element("XNivInuPrDifMresIbs"),
                        HNivInuPrDifMresIbs = (int)ins.Element("HNivInuPrDifMresIbs"),
                        XNivInuPhMresIbs = (decimal)ins.Element("XNivInuPhMresIbs"),
                        HNivInuPhMresIbs = (int)ins.Element("HNivInuPhMresIbs"),
                        XVarIdxPhJClV = (decimal)ins.Element("XVarIdxPhJClV"),
                        XNivInuPbMresIbs = (decimal)ins.Element("XNivInuPbMresIbs"),
                        HNivInuPbMresIbs = (int)ins.Element("HNivInuPbMresIbs"),
                        XVarIdxPbJClV = (decimal)ins.Element("XVarIdxPbJClV"),
                        XVarDrInuClV = (decimal)ins.Element("XVarDrInuClV"),
                        QDvdNetJValIbs = (decimal)ins.Element("QDvdNetJValIbs"),
                        QCapBsRfVIbs = (decimal)ins.Element("QCapBsRfVIbs"),
                        KAjCapBzIbs = (decimal)ins.Element("KAjCapBzIbs"),
                        XNivIrteNetClIbs = (decimal)ins.Element("XNivIrteNetClIbs")
                    }).ToList();
                }

                //    //Log.Information("TradeLastDay saved successfully");
                //}
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "There was a problem at SaveTradeLastDayAsync");
            }

            return list;
        }

        /// <summary>
        ///  تو این میتونیم تعداد سهام رو پیدا کنیم
        ///  numberOfShare
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<List<TseIndexB2>> GetIndexInstrumentAsync(CancellationToken cancellation = default)
        {
            var list = new List<TseIndexB2>();
            try
            {
                var client = new TsePublicV2SoapClient(EndpointConfiguration.TsePublicV2Soap12);
                var elements = new List<XElement>();

                foreach (var item in tseFlowMarkets)
                {

                    var data = await client.IndexInstrumentAsync(_tsetmcServiceSettings.User, _tsetmcServiceSettings.Password, 33878047680249697, 1);

                    if (data != null)
                    {
                        var tse = data.Nodes[1]?
                            .Element("TseIndexB1LastDay")?
                            .Elements("TseIndexB1LastDay")?
                            .ToList();

                        if (tse?.Count > 0)
                        {
                            elements.AddRange(tse);
                        }
                    }
                }
                if (elements.Any())
                {
                    list = elements.Select(ins => new TseIndexB2
                    {
                        InsCode = (long)ins.Element("InsCode"),
                        Date = DateTimeExtensions.StringToDateTime((string)ins.Element("DEven")),
                        Time = (int)ins.Element("HEven"),
                        XNivInuClMresIbs = (decimal)ins.Element("XNivInuClMresIbs"),
                        HNivInuClMresIbs = (int)ins.Element("HNivInuClMresIbs"),
                        XNivInuPrDifMresIbs = (decimal)ins.Element("XNivInuPrDifMresIbs"),
                        HNivInuPrDifMresIbs = (int)ins.Element("HNivInuPrDifMresIbs"),
                        XNivInuPhMresIbs = (decimal)ins.Element("XNivInuPhMresIbs"),
                        HNivInuPhMresIbs = (int)ins.Element("HNivInuPhMresIbs"),
                        XVarIdxPhJClV = (decimal)ins.Element("XVarIdxPhJClV"),
                        XNivInuPbMresIbs = (decimal)ins.Element("XNivInuPbMresIbs"),
                        HNivInuPbMresIbs = (int)ins.Element("HNivInuPbMresIbs"),
                        XVarIdxPbJClV = (decimal)ins.Element("XVarIdxPbJClV"),
                        XVarDrInuClV = (decimal)ins.Element("XVarDrInuClV"),
                        QDvdNetJValIbs = (decimal)ins.Element("QDvdNetJValIbs"),
                        QCapBsRfVIbs = (decimal)ins.Element("QCapBsRfVIbs"),
                        KAjCapBzIbs = (decimal)ins.Element("KAjCapBzIbs"),
                        XNivIrteNetClIbs = (decimal)ins.Element("XNivIrteNetClIbs")
                    }).ToList();
                }

                //    //Log.Information("TradeLastDay saved successfully");
                //}
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "There was a problem at SaveTradeLastDayAsync");
            }

            return list;
        }

        /// <summary>
        /// صنعت ها
        /// </summary>
        public async Task<List<TseIndexB2>> GetSectorAsync(CancellationToken cancellation = default)
        {
            var list = new List<TseIndexB2>();
            try
            {
                var client = new TsePublicV2SoapClient(EndpointConfiguration.TsePublicV2Soap12);
                var elements = new List<XElement>();

                foreach (var item in tseFlowMarkets)
                {
                    var data = await client.SectorAsync(_tsetmcServiceSettings.User, _tsetmcServiceSettings.Password);

                    if (data != null)
                    {
                        var tse = data.Nodes[1]?
                            .Element("SectorList")?
                            .Elements("TseSectorList")?
                            .ToList();

                        if (tse?.Count > 0)
                        {
                            elements.AddRange(tse);
                        }
                    }
                }
                if (elements.Any())
                {
                    list = elements.Select(ins => new TseIndexB2
                    {
                        InsCode = (long)ins.Element("InsCode"),
                        Date = DateTimeExtensions.StringToDateTime((string)ins.Element("DEven")),
                        Time = (int)ins.Element("HEven"),
                        XNivInuClMresIbs = (decimal)ins.Element("XNivInuClMresIbs"),
                        HNivInuClMresIbs = (int)ins.Element("HNivInuClMresIbs"),
                        XNivInuPrDifMresIbs = (decimal)ins.Element("XNivInuPrDifMresIbs"),
                        HNivInuPrDifMresIbs = (int)ins.Element("HNivInuPrDifMresIbs"),
                        XNivInuPhMresIbs = (decimal)ins.Element("XNivInuPhMresIbs"),
                        HNivInuPhMresIbs = (int)ins.Element("HNivInuPhMresIbs"),
                        XVarIdxPhJClV = (decimal)ins.Element("XVarIdxPhJClV"),
                        XNivInuPbMresIbs = (decimal)ins.Element("XNivInuPbMresIbs"),
                        HNivInuPbMresIbs = (int)ins.Element("HNivInuPbMresIbs"),
                        XVarIdxPbJClV = (decimal)ins.Element("XVarIdxPbJClV"),
                        XVarDrInuClV = (decimal)ins.Element("XVarDrInuClV"),
                        QDvdNetJValIbs = (decimal)ins.Element("QDvdNetJValIbs"),
                        QCapBsRfVIbs = (decimal)ins.Element("QCapBsRfVIbs"),
                        KAjCapBzIbs = (decimal)ins.Element("KAjCapBzIbs"),
                        XNivIrteNetClIbs = (decimal)ins.Element("XNivIrteNetClIbs")
                    }).ToList();
                }

                var ss = list.Where(e => e.InsCode == 33878047680249697).FirstOrDefault();

                //    //Log.Information("TradeLastDay saved successfully");
                //}
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "There was a problem at SaveTradeLastDayAsync");
            }

            return list;
        }

        /// <summary>
        /// زیر صنعت ها
        /// </summary>
        public async Task<List<TseIndexB2>> GetSubSectorAsync(CancellationToken cancellation = default)
        {
            var list = new List<TseIndexB2>();
            try
            {
                var client = new TsePublicV2SoapClient(EndpointConfiguration.TsePublicV2Soap12);
                var elements = new List<XElement>();

                foreach (var item in tseFlowMarkets)
                {
                    var data = await client.SubSectorAsync(_tsetmcServiceSettings.User, _tsetmcServiceSettings.Password);

                    if (data != null)
                    {
                        var tse = data.Nodes[1]?
                            .Element("TseIndexB2")?
                            .Elements("TseIndexB2")?
                            .ToList();

                        if (tse?.Count > 0)
                        {
                            elements.AddRange(tse);
                        }
                    }
                }
                if (elements.Any())
                {
                    list = elements.Select(ins => new TseIndexB2
                    {
                        InsCode = (long)ins.Element("InsCode"),
                        Date = DateTimeExtensions.StringToDateTime((string)ins.Element("DEven")),
                        Time = (int)ins.Element("HEven"),
                        XNivInuClMresIbs = (decimal)ins.Element("XNivInuClMresIbs"),
                        HNivInuClMresIbs = (int)ins.Element("HNivInuClMresIbs"),
                        XNivInuPrDifMresIbs = (decimal)ins.Element("XNivInuPrDifMresIbs"),
                        HNivInuPrDifMresIbs = (int)ins.Element("HNivInuPrDifMresIbs"),
                        XNivInuPhMresIbs = (decimal)ins.Element("XNivInuPhMresIbs"),
                        HNivInuPhMresIbs = (int)ins.Element("HNivInuPhMresIbs"),
                        XVarIdxPhJClV = (decimal)ins.Element("XVarIdxPhJClV"),
                        XNivInuPbMresIbs = (decimal)ins.Element("XNivInuPbMresIbs"),
                        HNivInuPbMresIbs = (int)ins.Element("HNivInuPbMresIbs"),
                        XVarIdxPbJClV = (decimal)ins.Element("XVarIdxPbJClV"),
                        XVarDrInuClV = (decimal)ins.Element("XVarDrInuClV"),
                        QDvdNetJValIbs = (decimal)ins.Element("QDvdNetJValIbs"),
                        QCapBsRfVIbs = (decimal)ins.Element("QCapBsRfVIbs"),
                        KAjCapBzIbs = (decimal)ins.Element("KAjCapBzIbs"),
                        XNivIrteNetClIbs = (decimal)ins.Element("XNivIrteNetClIbs")
                    }).ToList();
                }

                var ss = list.Where(e => e.InsCode == 33878047680249697).FirstOrDefault();

                //    //Log.Information("TradeLastDay saved successfully");
                //}
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "There was a problem at SaveTradeLastDayAsync");
            }

            return list;
        }

        public async Task<List<TseTradeLastDay>> GetIndexesAsync(CancellationToken cancellation = default)
        {
            // insCode شاخص کل
            // insCode ==> 32097828799138957

            //await GetSectorAsync(cancellation);
            //await GetSubSectorAsync(cancellation);

            await GetAllIndexAsync(cancellation);
            await GetIndexB1LastDayLastDataAsync(cancellation);


            await GetIndexB1LastDayOneInstAsync(cancellation);
            await GetIndexInstrumentAsync(cancellation);



            var list = new List<TseTradeLastDay>();
            try
            {
                var client = new TsePublicV2SoapClient(EndpointConfiguration.TsePublicV2Soap12);
                var elements = new List<XElement>();

                foreach (var item in tseFlowMarkets)
                {
                    // تاثیر روی شاخص
                    //var InstAffect = await client.InstAffectAsync(_tsetmcServiceSettings.User, _tsetmcServiceSettings.Password);

                    //var Sector = await client.SectorAsync(_tsetmcServiceSettings.User, _tsetmcServiceSettings.Password);

                    // اطلاعات شاخص ها 
                    // دارویی، خودرو، شاخص کل
                    //var IndexB1LastDayLastData = await client.IndexB1LastDayLastDataAsync(_tsetmcServiceSettings.User, _tsetmcServiceSettings.Password, item);

                    //var TOPA = await client.TOPAsync(_tsetmcServiceSettings.User, _tsetmcServiceSettings.Password, item );

                    //var IndexB2 = await client.IndexB2Async(_tsetmcServiceSettings.User, _tsetmcServiceSettings.Password, 20251215);

                    //if (tradeLastDay != null)
                    //{
                    //    var tse = tradeLastDay.Nodes[1].Elements("TradeLastDay").Elements("TradeLastDay").ToList();
                    //    elements.AddRange(tse);
                    //}
                }
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "There was a problem at SaveTradeLastDayAsync");
            }


            return list;
        }

    }
}


