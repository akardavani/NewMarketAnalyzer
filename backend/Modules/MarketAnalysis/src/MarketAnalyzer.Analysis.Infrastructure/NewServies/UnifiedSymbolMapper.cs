using MarketAnalyzer.Domain;
namespace MarketAnalyzer.Analysis.Infrastructure
{
    public static class UnifiedSymbolMapper
    {
        public static UnifiedSymbolRecord ToUnifiedSymbol(this CryptoSymbol source)
        {
            return new UnifiedSymbolRecord
            {
                MarketId = MarketType.Crypto,
                SymbolKey = source.Symbol,
                Symbol = source.Symbol,
                Name = source.SymbolCode,
                IsActive = source.Trading,
                MetadataJson = UnifiedSymbolJsonMapper.ToJson(new CryptoSymbolMetadata
                {
                    BaseCurrency = source.BaseCurrency,
                    QuoteCurrency = source.QuoteCurrency,
                    LastTradedPrice = source.LastTradedPrice,
                    Vol = source.Vol,
                    VolValue = source.VolValue
                }),
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static UnifiedSymbolRecord ToUnifiedSymbol(this ImeSymbol source)
        {
            return new UnifiedSymbolRecord
            {
                MarketId = MarketType.Ime,
                SymbolKey = source.Symbol,
                Symbol = source.Symbol,
                PersianName = source.PersianName,
                ParentSymbol = source.ParentSymbol,
                InsCode = source.Id.ToString(),
                IsActive = true,
                MetadataJson = UnifiedSymbolJsonMapper.ToJson(new ImeSymbolMetadata
                {
                    DeliveryDate = source.DeliveryDate,
                    PersianDeliveryDate = source.PersianDeliveryDate
                }),
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static UnifiedSymbolRecord ToUnifiedSymbol(this MacroSymbol source)
        {
            return new UnifiedSymbolRecord
            {
                MarketId = MarketType.Macro,
                SymbolKey = source.Symbol,
                Symbol = source.Symbol,
                Name = source.Symbol,
                PersianName = source.PersianName,
                ParentSymbol = source.ParentSymbol,
                IsActive = true,
                MetadataJson = UnifiedSymbolJsonMapper.ToJson(new MacroSymbolMetadata
                {
                }),
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static UnifiedSymbolRecord ToUnifiedSymbol(this CryptoTomanSymbol source)
        {
            return new UnifiedSymbolRecord
            {
                MarketId = MarketType.CryptoToman,
                SymbolKey = source.Symbol,
                Symbol = source.Symbol,
                Name = source.Symbol,
                PersianName = source.PersianName,
                ParentSymbol = source.ParentSymbol,
                IsActive = true,
                MetadataJson = UnifiedSymbolJsonMapper.ToJson(new CryptoTomanSymbolMetadata
                {
                }),
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static UnifiedSymbolRecord ToUnifiedSymbol(this TseSymbol source)
        {
            var insCode = source.InsCode.ToString();

            return new UnifiedSymbolRecord
            {
                MarketId = MarketType.Tse,
                SymbolKey = !string.IsNullOrWhiteSpace(source.InstrumentId)
                    ? source.InstrumentId
                    : insCode,
                Symbol = source.Symbol,
                InstrumentId = source.InstrumentId,
                InsCode = insCode,
                Name = source.Name,
                PersianName = source.Name,
                IsActive = true,
                MetadataJson = UnifiedSymbolJsonMapper.ToJson(new StockInstrumentMetadata
                {
                    InstrumentGroupId = source.InstrumentGroupId,
                    MarketSegment = source.MarketSegment,
                    BoardCode = source.BoardCode,
                    SectorCode = source.SectorCode,
                    BaseVol = source.BaseVol,
                    CIsin = source.CIsin,
                    CompanyName = source.CompanyName,
                    EnglishName = source.EnglishName,
                    Flow = source.Flow,
                    FourDigitCompanyCode = source.FourDigitCompanyCode,
                    InstrumentMnemonic = source.InstrumentMnemonic,
                    InstrumentType = source.InstrumentType,
                    SharesOrBondsNumber = source.SharesOrBondsNumber,
                    SubSectorCode = source.SubSectorCode,
                    Valid = source.Valid,
                    YVal = source.YVal,
                }),
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static UnifiedSymbolRecord ToUnifiedSymbol(this InvestmentFund source)
        {
            return new UnifiedSymbolRecord
            {
                MarketId = MarketType.InvestmentFund,
                SymbolKey = !string.IsNullOrWhiteSpace(source.FundInstrumentId)
                    ? source.FundInstrumentId
                    : source.RegNo,
                Symbol = source.RegNo,
                InstrumentId = source.FundInstrumentId,
                Name = source.Name,
                PersianName = source.Name,
                IsActive = true,
                MetadataJson = UnifiedSymbolJsonMapper.ToJson(new InvestmentFundMetadata
                {
                    FundType = source.FundType,
                    Date = source.Date,
                    FundPublisher = source.FundPublisher,
                    FundSize = source.FundSize,
                    InitiationDate = source.InitiationDate,
                    InvestedUnits = source.InvestedUnits,
                    IsCompleted = source.IsCompleted,
                    ManagerSeoRegisterNo = source.ManagerSeoRegisterNo,
                    NetAsset = source.NetAsset,
                    TypeOfInvest = source.TypeOfInvest,
                    WebsiteAddress = source.WebsiteAddress
                }),
                UpdatedAt = DateTime.UtcNow
            };
        }


        public static UnifiedSymbolRecord ToUnifiedSymbol(this RealEstateRegion source)
        {
            return new UnifiedSymbolRecord
            {
                MarketId = MarketType.RealEstate,
                SymbolKey = source.GlobalLocationId.ToString(),
                Symbol = source.GlobalLocationId.ToString(),
                Name = source.Zone,
                PersianName = source.Name,
                IsActive = true,
                MetadataJson = UnifiedSymbolJsonMapper.ToJson(new RealEstateRegionMetadata
                {
                    Province = source.Province,
                    ContinueUrl = source.ContinueUrl,
                    Region = source.Region,
                }),
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static UnifiedSymbolRecord ToUnifiedSymbol(this TseIndexSymbol source)
        {
            return new UnifiedSymbolRecord
            {
                MarketId = MarketType.TseIndex,
                SymbolKey = source.InstrumentId ?? source.InsCode,
                Symbol = source.SectorCode,
                InsCode = source.InsCode,
                Name = source.SectorName,
                InstrumentId = source.InstrumentId,
                PersianName = source.SectorName,
                IsActive = true,
                MetadataJson = UnifiedSymbolJsonMapper.ToJson(new TseTmcIndexMetadata
                {
                    Flow = source.Flow,
                }),
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}