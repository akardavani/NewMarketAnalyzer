using Microsoft.Extensions.Options;
using MarketAnalyzer.DataProvider.TseModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MarketAnalyzer.DataProvider
{
    public class TseClient
    {
        private readonly TseClientOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;

        public TseClient(IOptions<TseClientOptions> options, IHttpClientFactory httpClientFactory)
        {
            _options = options.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ICollection<TseBoard>> GetBoardsAsync()
        {
            var client = new TsePublicV2SoapClient(TsePublicV2SoapClient.EndpointConfiguration.TsePublicV2Soap12);
            var data = await client.BoardAsync(_options.UserName, _options.Password);
            if (data is not null && data.Nodes.Count > 1)
            {
                var doc = XDocument.Parse(data.Nodes[1].ToString());
                return doc.Descendants("BoardList")
                    .Descendants("TseBoardList")
                    .Select(element =>
                    {
                        return new TseBoard()
                        {
                            BoardCode = element.GetString("CComVal"),
                            BoardName = element.GetString("LBoard")
                        };
                    })
                    .ToList();
            }

            return null;
        }

        public async Task<ICollection<TseOptionContract>> GetOptionContractsAsync()
        {
            var client = new TsePublicV2SoapClient(TsePublicV2SoapClient.EndpointConfiguration.TsePublicV2Soap12);
            var data = await client.OptionAsync(_options.UserName, _options.Password);
            if (data is not null && data.Nodes.Count > 1)
            {
                var doc = XDocument.Parse(data.Nodes[1].ToString());
                return doc.Descendants("Option")
                    .Descendants("Option")
                    .Select(element =>
                    {
                        return new TseOptionContract()
                        {
                            InsCode = element.GetInt64("InsCode"),
                            UnderlyingAssetInsCode = element.GetInt64("UAInsCode"),
                            InstrumentId = element.GetString("InstrumentID"),
                            BuyOP = element.GetInt64("BuyOP"),
                            SellOP = element.GetInt64("SellOP"),
                            ContractSize = element.GetInt64("ContractSize"),
                            StrikePrice = element.GetDecimal("StrikePrice"),
                            BeginDate = element.GetDateOnly("BeginDate"),
                            EndDate = element.GetDateOnly("EndDate"),
                            AFactor = element.GetDecimal("AFactor"),
                            BFactor = element.GetDecimal("BFactor"),
                            CFactor = element.GetDecimal("CFactor"),
                            YesterdayOP = element.GetNullableInt64("YesterdayOP") ?? 0
                        };
                    })
                    .ToList();
            }

            return null;
        }

        public async Task<TseFutureInformation> GetFutureInformationAsync(string companyIsin)
        {
            var response = await GetAsync($"https://cdn.tsetmc.com/api/Instrument/GetInstrumentFutureByCIsin/{companyIsin}", default);
            if (!string.IsNullOrEmpty(response))
            {
                return JsonSerializer.Deserialize<TseFutureInformation>(response);
            }

            return null;
        }

        public async Task<ICollection<TseFutureInformation>> GetFutureInformationAsync(DateOnly date)
        {
            var deven = int.Parse(date.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
            var client = new TsePublicV2SoapClient(TsePublicV2SoapClient.EndpointConfiguration.TsePublicV2Soap12);
            var data = await client.FutureInformationAsync(_options.UserName, _options.Password, deven);
            if (data?.Any1 is not null)
            {
                var doc = XDocument.Parse(data.Any1.OuterXml);
                return doc.Descendants("TseAlert")
                    .Descendants("TseAlert")
                    .Select(element =>
                    {
                        return new TseFutureInformation()
                        {
                            Id = element.GetInt64("ID"),
                            CompanyIsin = element.GetString("Cisin"),
                            BaseInstrumentCompanyIsin = element.GetString("BaseInstrumentCIsin"),
                            Quantity = element.GetInt32("Quantity"),
                            MaxLegalClientOpenPositions = element.GetNullableInt64("MaxClientOP"),
                            MaxNaturalClientOpenPositions = element.GetNullableInt64("MaxActClientOP"),
                            MaxBrokerOpenPositions = element.GetInt64("MaxBrokerOP"),
                            MaxMarketOpenPositions = element.GetInt64("MaxMarketOP"),
                            StratDate = element.GetNullableDateOnly("BeginDate"),
                            EndDate = element.GetDateOnly("EndDate"),
                            PhysicalSettlementDate = element.GetNullableDateOnly("PhysicalSettlementDate"),
                            PrioritizationSettlementDate = element.GetNullableDateOnly("PrioritizationstlmntDate"),
                            CashSettlementDate = element.GetNullableDateOnly("CashStlmntDate"),
                            InitialMarginValue = element.GetDecimal("IMAValue"),
                            RequiredMarginValue = element.GetDecimal("RMBValue"),
                            RoundingCoefficient = element.GetInt64("CRound_Value"),
                            ExtraMargin = element.GetInt64("DExtraMargin"),
                            MaintananceMarginPercent = element.GetDecimal("MaintananceMarginPercent"),
                            PartialSettlementAllowed = element.GetBoolean("PartialStlmntIsAllowded"),
                            PersianMonth = element.GetInt32("Month"),
                            PersianYear = element.GetInt32("Year"),
                            SettlementTypeId = element.GetInt32("SettlementTypeID"),
                            SettlemnetTypeName = element.GetString("SettlemnetTypeName"),
                            AppropriationId = element.GetInt32("AppropiationID"),
                            AppropriationName = element.GetString("AppropriationName"),
                            BaseAssetPriceDeterminationId = element.GetInt32("BaseAssetPriceDeterminationID"),
                            PenaltyRatio = element.GetDecimal("PenaltyRatio"),
                            BaseAssetPriceName = element.GetString("BaseAssetPriceName"),
                            PhysicalFeeRatio = element.GetDecimal("PhysicalFeeRatio"),
                            CashFeeRatio = element.GetDecimal("CashFeeRatio"),
                            TaxRatio = element.GetDecimal("TaxRatio"),
                            OpenPositions = element.GetNullableInt32("MOP"),
                        };
                    })
                    .ToList();
            }

            return null;
        }

        public async Task<ICollection<TseSector>> GetSectorsAsync()
        {
            var client = new TsePublicV2SoapClient(TsePublicV2SoapClient.EndpointConfiguration.TsePublicV2Soap12);
            var data = await client.SectorAsync(_options.UserName, _options.Password);
            if (data is not null && data.Nodes.Count > 1)
            {
                var doc = XDocument.Parse(data.Nodes[1].ToString());
                return doc.Descendants("SectorList")
                    .Descendants("TseSectorList")
                    .Select(element =>
                    {
                        return new TseSector()
                        {
                            SectorCode = element.GetString("CSecVal"),
                            SectorName = element.GetString("LSecVal")
                        };
                    })
                    .ToList();
            }

            return null;
        }

        public async Task<ICollection<TseSubSector>> GetSubSectorsAsync()
        {
            var client = new TsePublicV2SoapClient(TsePublicV2SoapClient.EndpointConfiguration.TsePublicV2Soap12);
            var data = await client.SubSectorAsync(_options.UserName, _options.Password);
            if (data is not null && data.Nodes.Count > 1)
            {
                var doc = XDocument.Parse(data.Nodes[1].ToString());
                return doc.Descendants("SubSector")
                    .Descendants("SubSector")
                    .Select(element =>
                    {
                        return new TseSubSector()
                        {
                            SectorCode = element.GetString("CSecVal"),
                            SubSectorCode = element.GetString("CSoSecVal"),
                            SubSectorName = element.GetString("LSoSecVal")
                        };
                    })
                    .ToList();
            }

            return null;
        }

        /// <summary>
        /// اطلاعات سابقه شاخص ها را ارائه مي کند. 
        /// </summary>
        /// <param name="date">تاريخ</param>
        /// <returns></returns>
        public async Task<ICollection<TseIndexInfo>> GetIndexInfoAsync(DateTime? date = null)
        {
            date ??= DateTime.Today;
            var deven = int.Parse(date.Value.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
            var client = new TsePublicV2SoapClient(TsePublicV2SoapClient.EndpointConfiguration.TsePublicV2Soap12);
            var data = await client.IndexB2Async(_options.UserName, _options.Password, deven);
            if (data is not null && data.Nodes.Count > 1)
            {
                var doc = XDocument.Parse(data.Nodes[1].ToString());
                return doc.Descendants("TseIndexB2")
                    .Descendants("TseIndexB2")
                    .Select(e =>
                    {
                        return new TseIndexInfo()
                        {
                            InsCode = e.GetInt64("InsCode"),
                            DateTime = e.GetDateTime("DEven", "HEven"),
                            LastValue = e.GetDecimal("XNivInuClMresIbs"),
                            FirstValue = e.GetDecimal("XNivInuPrDifMresIbs"),
                            FirstValueTime = e.GetTime("HNivInuPrDifMresIbs"),
                            HighestValue = e.GetDecimal("XNivInuPhMresIbs"),
                            HighestValueChangePercent = e.GetDecimal("XVarIdxPhJClV"),
                            HighestValueTime = e.GetTime("HNivInuPhMresIbs"),
                            LowestValue = e.GetDecimal("XNivInuPbMresIbs"),
                            LowestValueChangePercent = e.GetDecimal("XVarIdxPbJClV"),
                            LowestValueTime = e.GetTime("HNivInuPbMresIbs"),
                            AdjustmentCoefficient = e.GetDecimal("KAjCapBzIbs"),
                            LastDayCapital = e.GetDecimal("QCapBsRfVIbs"),
                            InstrumentsDividend = e.GetDecimal("QDvdNetJValIbs"),
                            NetCashValue = e.GetDecimal("XNivIrteNetClIbs"),
                            InstrumentsDecreasedChangePercent = e.GetDecimal("XVarDrInuClV")
                        };
                    })
                .ToList();
            }

            return null;
        }

        /// <summary>
        /// اطلاعات معاملات روزانه را ارائه مي کند.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="flow"></param>
        /// <returns></returns>
        public async Task<ICollection<TsePriceHistory>> GetPriceHistoryAsync(DateOnly date, DataFlow flow)
        {
            var deven = int.Parse(date.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
            var client = new TsePublicV2SoapClient(TsePublicV2SoapClient.EndpointConfiguration.TsePublicV2Soap12);
            var data = await client.TradeOneDayAsync(_options.UserName, _options.Password, deven, (byte)flow);
            if (data is not null && data.Nodes.Count > 1)
            {
                var doc = XDocument.Parse(data.Nodes[1].ToString());
                return doc.Descendants("TradeSelectedDate")
                    .Descendants("TradeSelectedDate")
                    .Select(e =>
                    {
                        return new TsePriceHistory()
                        {
                            InsCode = e.GetInt64("InsCode"),
                            ClosingPrice = e.GetDecimal("PClosing"),
                            EntryDate = e.GetDateOnly("DEven"),
                            FirstTradePrice = e.GetDecimal("PriceFirst"),
                            HighestTradePrice = e.GetDecimal("PriceMax"),
                            LastDayClosingPrice = e.GetDecimal("PriceYesterday"),
                            LastTradePrice = e.GetDecimal("PDrCotVal"),
                            LowestTradePrice = e.GetDecimal("PriceMin"),
                            TotalTradedQuantity = e.GetInt64("QTotTran5J"),
                            TotalTradesNumber = e.GetInt64("ZTotTran"),
                            TotalTradeValue = e.GetDecimal("QTotCap"),
                        };
                    })
                .ToList();
            }

            return null;
        }

        /// <summary>
        ///  اطلاعات مربوط به نمادها را ارائه مي کند
        /// </summary>
        /// <param name="flow"></param>
        /// <returns></returns>
        public async Task<ICollection<TseInstrument>> GetInstrumentsAsync(DataFlow flow)
        {
            var client = new TsePublicV2SoapClient(TsePublicV2SoapClient.EndpointConfiguration.TsePublicV2Soap12);
            var data = await client.InstrumentAsync(_options.UserName, _options.Password, (byte)flow);
            if (data is not null && data.Nodes.Count > 1)
            {
                var doc = XDocument.Parse(data.Nodes[1].ToString());
                return doc.Descendants("Instruments")
                    .Descendants("TseInstruments")
                    .Select(e => new TseInstrument()
                    {
                        InstrumentId = e.GetString("InstrumentID"),
                        CompanyIsin = e.GetString("CIsin"),
                        Name = e.GetString("LVal18"),
                        FullName = e.GetString("LVal30"),
                        NativeName = e.GetString("LVal18AFC"),
                        CompanyName = e.GetString("LSoc30"),
                        CompanyCode = e.GetString("CSocCSAC"),
                        Mnemonic = e.GetString("CValMne"),
                        InstrumentClass = e.GetString("CGdSVal"),
                        InstrumentType = e.GetInt32("YVal"),
                        MarketSegment = e.GetString("YMarNSC"),
                        InstrumentGroupId = e.GetString("CGrValCot"),
                        BoardCode = e.GetString("CComVal"),
                        SectorCode = e.GetString("CSecVal"),
                        SubSectorCode = e.GetString("CSoSecVal"),
                        SettlementDelayType = e.GetInt32("YDeComp"),
                        SharesOrBondsNumber = e.GetDecimal("ZTitad"),
                        MinimumEnterableQuantity = e.GetInt64("QtitMinSaiOmProd"),
                        MaximumnterableQuantity = e.GetInt64("QtitMaxSaiOmProd"),
                        ParValue = e.GetDecimal("QNmVlo"),
                        FixedPriceTick = e.GetDecimal("QPasCotFxeVal"),
                        LotSize = e.GetInt64("QQtTranMarVal"),
                        BaseVolume = e.GetInt64("BaseVol"),
                        FirstTradingDate = e.GetDateTime("DInMar"),
                        UnitPriceType = e.GetByte("YUniExpP"),
                        HighIntermediateThreshold = e.GetDecimal("PSaiSMaxOkValMdv"),
                        LowIntermediateThreshold = e.GetDecimal("PSaiSMinOkValMdv"),
                        EventType = e.GetString("YOPSJ"),
                        EventDateTime = e.GetDateTime("DEVen"),
                        InsCode = e.GetInt64("InsCode"),
                        IsActive = e.GetBoolean("Valid"),
                        Flow = e.GetFlow("Flow")
                    })
                .ToList();
            }

            return null;
        }

        private async Task<string> GetAsync(string url, CancellationToken cancellation = default)
        {
            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            var request = await client.GetAsync(url, cancellation);
            request.EnsureSuccessStatusCode();
            var response = await request.Content.ReadAsStreamAsync(cancellation);
            using var decompressor = new GZipStream(response, CompressionMode.Decompress);
            using var reader = new StreamReader(decompressor);
            return await reader.ReadToEndAsync(cancellation);
        }
    }
}