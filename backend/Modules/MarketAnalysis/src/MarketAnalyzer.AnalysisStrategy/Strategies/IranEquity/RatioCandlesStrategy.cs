using MarketAnalyzer.Domain;
using MarketAnalyzer.Analysis.Infrastructure;
using MarketAnalyzer.SharedKernel;
using System.Text.Json;

namespace MarketAnalyzer.AnalysisStrategy
{
    public partial class RatioCandlesStrategy : IAnalysisStrategy
    {
        private readonly IEnumerable<IMarketService> _marketServices;
        private readonly IUnifiedSymbolRepository _unifiedSymbolRepository;
        private readonly IUnifiedCandleQueryService _candleQueryService;
        private readonly IUnifiedSymbolQueryService _symbolQueryService;

        private const string SymbolParameter = "Symbols";
        private const string TimeframeParameter = "Timeframe";
        private const string StartDateParameter = "StartDate";
        private const string EndDateParameter = "EndDate";

        public string Name => StrategyName.RatioCandles;

        public RatioCandlesStrategy(
            IEnumerable<IMarketService> marketServices,
            IUnifiedSymbolRepository unifiedSymbolRepository,
            IUnifiedCandleQueryService candleQueryService,
            IUnifiedSymbolQueryService symbolQueryService)
        {
            _marketServices = marketServices;
            _unifiedSymbolRepository = unifiedSymbolRepository;
            _candleQueryService = candleQueryService;
            _symbolQueryService = symbolQueryService;
        }

        public async Task<AnalysisResult> ExecuteAsync(AnalysisRequest request,  CancellationToken cancellation)
        {
            var symbols = ParameterHelper.GetRequiredStringListParameter(request, SymbolParameter);
            var calculateType = ParameterHelper.GetRequiredParameter<string>(request, TimeframeParameter);
            var startDate = ParameterHelper.GetRequiredParameter<DateTime>(request, StartDateParameter);
            var endDate = ParameterHelper.GetRequiredParameter<DateTime>(request, EndDateParameter);

            //var query = new MarketQueryBuilder()
            //    .Between(startDate, endDate)
            //    .WithTimeframe(TimeframeDto.Daily)
            //    .ForSymbols(symbols.ToArray() ?? null)
            //    .Build();

            //var candles = await GetAllCandels(query, cancellation);

            //var candles1 = await _coreService
            //    .GetCandlesAsync(query, null, cancellation);

            var candles = await _candleQueryService.GetCandlesAsync(
               new CandleQuery
               {
                   From = startDate,
                   To = endDate,
                   Timeframe = Timeframe.Daily,
                   SymbolKeys = symbols,
                   Markets = new[]
                   {
                        //MarketType.Tse,
                        MarketType.Macro,
                        MarketType.Crypto,
                        MarketType.TseIndex,
                        MarketType.CryptoToman
                   }
               },
               cancellation);

            //var allSymbol1 = await _coreService.GetSymbols(null, cancellation);

            var tseSymbols = await GetTseSymbol();

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
            cancellation);

            var AllSymbols = tseSymbols
                .Union(allSymbol)
                .ToList();

            // 1) محور مشترک: PersianDate یکتا + یک Date نماینده (مثلاً آخرین)
            var allDates = candles
                .GroupBy(c => c.PersianDate)
                .Select(g => new
                {
                    PersianDate = g.Key,
                    Date = g.Max(x => x.Date) // نماینده روز (آخرین کندل)
                })
                .OrderBy(x => x.Date)
                .ToList();

            var allresults = new List<CandleSeriesResult>();

            // 2) برای هر نماد، یک نگاشت PersianDate → Candle (یک رکورد در روز)
            foreach (var symbol in symbols)
            {
                // اولین قیمت بازه
                var firstPriceOfPersiod = candles
                    .Where(e => e.SymbolKey == symbol)
                    .OrderBy(e => e.Date)
                    .FirstOrDefault()?.Close ?? 1;

                var dailyForSymbol = candles
                    .Where(e => e.SymbolKey == symbol)
                    .GroupBy(e => e.PersianDate)
                    .Select(g => g.OrderByDescending(x => x.Date).First()) // یک رکورد در روز
                    .ToDictionary(e => e.PersianDate, e => e);

                var results = new List<CandleResult>();

                decimal? lastGrowth = null;
                // 3) هم‌تراز با محور مشترک: اگر داده نیست، Growth = null
                foreach (var d in allDates)
                {
                    // نگه‌دارنده آخرین مقدار Growth
                    if (dailyForSymbol.TryGetValue(d.PersianDate, out var c))
                    {
                        var growth = c.Close / firstPriceOfPersiod;
                        results.Add(new CandleResult
                        {
                            Date = c.Date.ToDateOnly(),
                            PersianDate = c.PersianDate,
                            //Growth = c.Close 
                            Growth = growth
                        });
                        lastGrowth = growth;
                    }
                    else
                    {
                        results.Add(new CandleResult
                        {
                            Date = d.Date.ToDateOnly(),
                            PersianDate = d.PersianDate,
                            Growth = lastGrowth ?? 0 // Chart.js با null خط را قطع می‌کند و محور مشترک حفظ می‌شود
                        });
                    }
                }

                allresults.Add(new CandleSeriesResult
                {
                    InstrumentId = symbol,
                    Symbol = AllSymbols.Where(e => e.SymbolKey == symbol).FirstOrDefault().SymbolKey,
                    Results = results
                });
            }

            return new AnalysisResult
            {
                Strategy = Name,
                Data = allresults

            };
        }

        //public async Task<List<MarketCandle>> GetAllCandels(MarketQuery query, CancellationToken cancellation)
        //{
        //    var tasks = _marketServices.Select(service =>
        //        service.GetCandlesAsync(query, cancellation));

        //    var results = await Task.WhenAll(tasks);

        //    return results.SelectMany(x => x).ToList();
        //}

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
    }
}
