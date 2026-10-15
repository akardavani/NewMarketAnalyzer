using MarketAnalyzer.Domain;
using MarketAnalyzer.Analysis.Infrastructure;
using System.Globalization;
using System.Text.Json;

namespace MarketAnalyzer.UI.Web
{
    public class StrategyOptionProvider
    {
        private readonly IUnifiedSymbolQueryService _symbolQueryService;
        private readonly IUnifiedSymbolRepository _unifiedSymbolRepository;

        public StrategyOptionProvider(IUnifiedSymbolQueryService symbolQueryService, IUnifiedSymbolRepository unifiedSymbolRepository)
        {
            _symbolQueryService = symbolQueryService;
            _unifiedSymbolRepository = unifiedSymbolRepository;
        }

        public Dictionary<string, string> AllSymbols { get; set; } = new();

        public Dictionary<string, string> Markets { get; set; } = Enum.GetValues<MarketType>()
            .Where(m => m is MarketType.Tse or MarketType.Macro or MarketType.Crypto or MarketType.TseIndex)
            .ToDictionary(
                m => m.GetName(),
                m => m.GetCode()
            );

        public Dictionary<string, string> Timeframes { get; set; } = Enum.GetValues<Timeframe>()
            .ToDictionary(
                t => t.GetName(),
                t => t.GetCode()
            );


        public List<StrategyParameterOption> GetMarketOptions()
        {
            return Markets
                .Select(x => new StrategyParameterOption
                {
                    Text = x.Key,
                    Value = x.Value
                })
                .ToList();
        }

        public List<StrategyParameterOption> GetTimeframeOptions()
        {
            return Timeframes
                .Select(x => new StrategyParameterOption
                {
                    Text = x.Key,
                    Value = x.Value
                })
                .ToList();
        }

        private async Task<List<UnifiedSymbolRecord>> GetTseSymbol()
        {
            var symbols = await _unifiedSymbolRepository.GetAllAsync(MarketType.Tse, CancellationToken.None);

            var symbolKeys = symbols
                .Where(e => e.InstrumentId.EndsWith("0001") &&
                            e.IsActive)
                .AsEnumerable() // از اینجا به بعد LINQ در حافظه اجرا می‌شود
                .Where(e =>
                {
                    if (string.IsNullOrWhiteSpace(e.MetadataJson))
                        return false;

                    var meta = JsonSerializer.Deserialize<StockInstrumentMetadata>(e.MetadataJson);

                    return meta != null
                        //&& !string.IsNullOrEmpty(meta.InstrumentGroupId)
                        //&& meta.InstrumentGroupId.Contains("M1")
                        && meta.SubSectorCode != "6812"
                        && meta.Flow != 4;
                })
                //.Select(x => x.SymbolKey)
                .Distinct()
                .ToList();

            return symbolKeys;
        }
        public async Task<List<StrategyParameterOption>> GetSymbolOptions()
        {
            var tseSymbols = await GetTseSymbol();
            //var cryptoSymbols = await _unifiedSymbolRepository.GetAllAsync(MarketType.Crypto, CancellationToken.None);
            //var macroSymbols = await _unifiedSymbolRepository.GetAllAsync(MarketType.Macro, CancellationToken.None);
            //var tseIndexSymbols = await _unifiedSymbolRepository.GetAllAsync(MarketType.TseIndex, CancellationToken.None);
            //var investmentFundSymbols = await _unifiedSymbolRepository.GetAllAsync(MarketType.InvestmentFund, CancellationToken.None);

            //var AllSymbols = tseIndexSymbols
            //    .Union(cryptoSymbols)
            //    .Union(macroSymbols)
            //    .Union(tseIndexSymbols)
            //    .Union(investmentFundSymbols)
            //    .ToList();

            var allSymbol = await _symbolQueryService.GetSymbolsAsync(
                new SymbolQuery
                {
                    Markets = new[]
                    {
                        //MarketType.Tse,
                        MarketType.Macro,
                        MarketType.Crypto,
                        MarketType.TseIndex,
                        MarketType.CryptoToman,
                    },
                    IsActive = true
                },
             CancellationToken.None);

            var AllSymbols = tseSymbols
                .Union(allSymbol)
                .ToList();

            //AllSymbols = await _coreService
            //    .GetSymbols(new[] { MarketTypeName.Stock, 
            //        MarketTypeName.Crypto,
            //        MarketTypeName.Macro,
            //        MarketTypeName.InvestmentFund,
            //        MarketTypeName.TseTmcIndex, 
            //        //MarketTypeName.RealEstate, 
            //        //MarketTypeName.IME,
            //        },
            //        CancellationToken.None);

            var ss = AllSymbols
                .Select(x => new StrategyParameterOption
                {
                    Text = x.PersianName ?? x.SymbolKey,
                    Value = x.SymbolKey
                })
                .ToList();

            return ss;
            //return AllSymbols
            //    .Select(x => new StrategyParameterOption
            //    {
            //        Text = x.Value,
            //        Value = x.Key
            //    })
            //    .ToList();
        }

        public string TodayShamsi()
        {
            var pc = new PersianCalendar();
            var now = DateTime.Now;

            return $"{pc.GetYear(now):0000}-{pc.GetMonth(now):00}-{pc.GetDayOfMonth(now):00}";
        }

        public string DaysAgoShamsi(int days)
        {
            var pc = new PersianCalendar();
            var dt = DateTime.Now.AddDays(-days);

            return $"{pc.GetYear(dt):0000}-{pc.GetMonth(dt):00}-{pc.GetDayOfMonth(dt):00}";
        }
    }
}
