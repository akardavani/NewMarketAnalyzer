using MarketAnalyzer.MarketIntelligence.Domain;
namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public static class UnifiedSymbolReverseMapper
    {
        public static TseSymbol ToStockInstrument(this UnifiedSymbolRecord source)
        {
            var metadata = UnifiedSymbolJsonMapper
                .FromJson<StockInstrumentMetadata>(source.MetadataJson);

            return new TseSymbol
            {
                Symbol = source.Symbol,
                InstrumentId = source.InstrumentId,
                InsCode = long.TryParse(source.InsCode, out var id) ? id : 0,
                Name = source.Name,
                InstrumentGroupId = metadata?.InstrumentGroupId,
                MarketSegment = metadata?.MarketSegment,
                BoardCode = metadata?.BoardCode ?? 0,
                SectorCode = metadata?.SectorCode,
                BaseVol = metadata?.BaseVol ?? 0,
                CIsin = metadata?.CIsin,
                CompanyName = metadata?.CompanyName,
                EnglishName = metadata?.EnglishName,
                Flow = metadata?.Flow ?? 0,
                FourDigitCompanyCode = metadata?.FourDigitCompanyCode,
                InstrumentMnemonic = metadata?.InstrumentMnemonic,
                InstrumentType = metadata?.InstrumentType ?? 0,
                SharesOrBondsNumber = metadata?.SharesOrBondsNumber ?? 0,
                SubSectorCode = metadata?.SubSectorCode,
                Valid = metadata?.Valid ?? 0,
                YVal = metadata?.YVal
            };
        }

        public static CryptoSymbol ToCryptoSymbol(this UnifiedSymbolRecord source)
        {
            var metadata = UnifiedSymbolJsonMapper
                .FromJson<CryptoSymbolMetadata>(source.MetadataJson);

            return new CryptoSymbol
            {
                Symbol = source.SymbolKey,
                SymbolCode = source.Name,
                Trading = source.IsActive,
                BaseCurrency = metadata?.BaseCurrency,
                QuoteCurrency = metadata?.QuoteCurrency,
                LastTradedPrice = metadata?.LastTradedPrice ?? 0,
                Vol = metadata?.Vol ?? 0,
                VolValue = metadata?.VolValue ?? 0
            };
        }

        public static ImeSymbol ToImeSymbol(this UnifiedSymbolRecord source)
        {
            var metadata = UnifiedSymbolJsonMapper
                .FromJson<ImeSymbolMetadata>(source.MetadataJson);

            return new ImeSymbol
            {
                Symbol = source.SymbolKey,
                PersianName = source.PersianName,
                ParentSymbol = source.ParentSymbol,
                Id = long.TryParse(source.InsCode, out var id) ? id : 0,
                DeliveryDate = metadata?.DeliveryDate ?? new DateOnly(),
                PersianDeliveryDate = metadata?.PersianDeliveryDate
            };
        }

        public static MacroSymbol ToMacroSymbol(this UnifiedSymbolRecord source)
        {
            return new MacroSymbol
            {
                Symbol = source.SymbolKey,
                PersianName = source.PersianName,
                ParentSymbol = source.ParentSymbol
            };
        }

        public static CryptoTomanSymbol ToCryptoTomanSymbol(this UnifiedSymbolRecord source)
        {
            return new CryptoTomanSymbol
            {
                Symbol = source.SymbolKey,
                PersianName = source.PersianName,
                ParentSymbol = source.ParentSymbol
            };
        }

        public static InvestmentFund ToInvestmentFund(this UnifiedSymbolRecord source)
        {
            var metadata = UnifiedSymbolJsonMapper
                .FromJson<InvestmentFundMetadata>(source.MetadataJson);

            return new InvestmentFund
            {
                RegNo = source.Symbol,
                FundInstrumentId = source.InstrumentId,
                Name = source.Name,
                FundType = metadata?.FundType,
                Date = metadata?.Date,
                FundPublisher = metadata?.FundPublisher,
                FundSize = metadata?.FundSize,
                InitiationDate = metadata?.InitiationDate,
                InvestedUnits = metadata?.InvestedUnits,
                IsCompleted = metadata?.IsCompleted ?? false,
                ManagerSeoRegisterNo = metadata?.ManagerSeoRegisterNo,
                NetAsset = metadata?.NetAsset,
                TypeOfInvest = metadata?.TypeOfInvest,
                WebsiteAddress = metadata?.WebsiteAddress
            };
        }

        public static RealEstateRegion ToRealEstateRegion(this UnifiedSymbolRecord source)
        {
            var metadata = UnifiedSymbolJsonMapper
                .FromJson<RealEstateRegionMetadata>(source.MetadataJson);

            return new RealEstateRegion
            {
                GlobalLocationId = long.Parse(source.SymbolKey),
                Name = source.PersianName,
                Zone = source.Name,
                Province = metadata?.Province,
                ContinueUrl = metadata?.ContinueUrl,
                Region = metadata?.Region
            };
        }
        public static TseIndexSymbol ToTseTmcIndex(this UnifiedSymbolRecord source)
        {
            var metadata = UnifiedSymbolJsonMapper
                .FromJson<TseTmcIndexMetadata>(source.MetadataJson);

            return new TseIndexSymbol
            {
                InstrumentId = source.InstrumentId,
                InsCode = source.InsCode,
                SectorName = source.Name,
                SectorCode = source.Symbol,
                Flow = metadata?.Flow ?? 0
            };
        }

    }
}