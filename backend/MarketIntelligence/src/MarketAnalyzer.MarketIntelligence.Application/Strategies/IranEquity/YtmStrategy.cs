using MarketAnalyzer.MarketIntelligence.Domain;
using MarketAnalyzer.MarketIntelligence.Infrastructure;
using MarketAnalyzer.SharedKernel;

namespace MarketAnalyzer.MarketIntelligence.Application
{
    public class YtmStrategy : IAnalysisStrategy
    {
        private const string SymbolParameter = "Symbol";
        private const string CalculatesParameter = "Calculates";
        private const string StartDateParameter = "StartDate";
        private const string EndDateParameter = "EndDate";

        // طلا لوتوس
        // "ETC", "IRTKLOTF0001";
        // سپرده نقره
        // "SIL", "BaseIme-Silver";
        private const string EtcSymbolPrefix = "ETC";
        private const string SilverSymbolPrefix = "SIL";

        private readonly IStockService _stockService;
        private readonly IUnifiedCandleQueryService _candleQueryService;
        private readonly IUnifiedSymbolRepository _unifiedSymbolRepository;
        private readonly ICalculateImeContractIndex _calculateImeContractIndex;

        public string Name => StrategyName.YtmFuture;

        public YtmStrategy(
            IStockService stockService,
            IUnifiedCandleQueryService candleQueryService,
             IUnifiedSymbolRepository unifiedSymbolRepository,
            ICalculateImeContractIndex calculateImeContractIndex)
        {
            _stockService = stockService;
            _candleQueryService = candleQueryService;
            _unifiedSymbolRepository = unifiedSymbolRepository;
            _calculateImeContractIndex = calculateImeContractIndex;
        }

        public async Task<AnalysisResult> ExecuteAsync(
            AnalysisRequest request,
            CancellationToken cancellation)
        {
            var symbolStart = ParameterHelper.GetRequiredParameter<string>(request, SymbolParameter);
            var calculateType = ParameterHelper.GetRequiredParameter<string>(request, CalculatesParameter);
            var startDate = ParameterHelper.GetRequiredParameter<DateTime>(request, StartDateParameter);
            var endDate = ParameterHelper.GetRequiredParameter<DateTime>(request, EndDateParameter);

            var chartData = await CreateChartDataAsync(
                calculateType,
                symbolStart,
                startDate,
                endDate,
                cancellation);

            return new AnalysisResult
            {
                Strategy = Name,
                Data = chartData
            };
        }

        private async Task<List<CandleResult>> CreateChartDataAsync(
            string calculateType,
            string symbolStart,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellation)
        {
            if (calculateType.Contains("Ytm"))
            {
                return await CreateYtmAsync(
                    calculateType,
                    symbolStart,
                    startDate,
                    endDate,
                    cancellation);
            }

            if (calculateType.Contains("Sood"))
            {
                return await CreateSoodAsync(
                    symbolStart,
                    startDate,
                    endDate,
                    cancellation);
            }

            return await CreateIndexAsync(
                calculateType,
                symbolStart,
                startDate,
                endDate,
                cancellation);
        }

        private async Task<List<CandleResult>> CreateIndexAsync(
            string searchType,
            string symbolStart,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellation)
        {
            var futureCandles = await GetIndexCandlesAsync(
                searchType,
                symbolStart,
                startDate,
                endDate,
                cancellation);

            var baseCandles = await GetBaseCandlesAsync(
                symbolStart,
                startDate,
                endDate,
                cancellation);

            return CalculateRatio.Calculate(futureCandles, baseCandles, true);
        }

        private async Task<List<CandleResult>> CreateYtmAsync(
            string searchType,
            string symbolStart,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellation)
        {
            var futureCandles = await GetIndexCandlesAsync(
                searchType,
                symbolStart,
                startDate,
                endDate,
                cancellation);

            var imeSymbols = await GetImeSymbolsAsync(symbolStart, cancellation);

            var symbolMap = imeSymbols
                .GroupBy(x => x.Symbol)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(x => x.DeliveryDate).First());

            var futureYtmCandles = BuildFutureYtmCandles(futureCandles, symbolMap);

            var baseCandles = await GetBaseCandlesAsync(
                symbolStart,
                startDate,
                endDate,
                cancellation);

            var spotYtmCandles = BuildSpotYtmCandles(baseCandles);

            return CalculateYTM.Calculate(futureYtmCandles, spotYtmCandles, true);
        }

        private async Task<List<CandleResult>> CreateSoodAsync(
            string symbolStart,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellation)
        {
            var futureCandles = await _calculateImeContractIndex
                .CreateLatestDueDateContractIndex(
                    symbolStart,
                    startDate,
                    endDate,
                    cancellation);

            var baseCandles = await GetBaseCandlesAsync(
                symbolStart,
                startDate,
                endDate,
                cancellation);

            return BuildSoodChartData(futureCandles, baseCandles);
        }

        private async Task<List<MarketCandle>> GetIndexCandlesAsync(
            string searchType,
            string symbolStart,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellation)
        {
            if (searchType.Contains("Nearest"))
            {
                return await _calculateImeContractIndex
                    .CreateNearestDueDateContractIndex(
                        symbolStart,
                        startDate,
                        endDate,
                        cancellation);
            }

            if (searchType.Contains("Second"))
            {
                return await _calculateImeContractIndex
                    .CreateSecondDueDateContractIndex(
                        symbolStart,
                        startDate,
                        endDate,
                        cancellation);
            }

            if (searchType.Contains("Latest"))
            {
                return await _calculateImeContractIndex
                    .CreateLatestDueDateContractIndex(
                        symbolStart,
                        startDate,
                        endDate,
                        cancellation);
            }

            return new List<MarketCandle>();
        }

        private async Task<List<MarketCandle>> GetBaseCandlesAsync(
            string symbolStart,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellation)
        {
            var imeSymbols = await GetImeSymbolsAsync(symbolStart, cancellation);

            var parentSymbol = imeSymbols
                .OrderBy(x => x.DeliveryDate)
                .FirstOrDefault()
                ?.ParentSymbol;

            if (string.IsNullOrWhiteSpace(parentSymbol))
                return new List<MarketCandle>();

            var query = new MarketQueryBuilder()
                .Between(startDate, endDate)
                .ForSymbols(parentSymbol)
                .Build();

            var baseSymbol = await _candleQueryService.GetCandlesAsync(
                new CandleQuery
                {
                    From = startDate,
                    To = endDate,
                    Timeframe = Timeframe.Daily,
                    SymbolKeys = new List<string> { parentSymbol },
                    Markets = new[]
                    {
                        MarketType.Tse,
                        MarketType.Ime
                    }
                },
                cancellation);

            return baseSymbol
                .Select(e => new MarketCandle(e))
                .ToList();

            //return symbolStart switch
            //{
            //    EtcSymbolPrefix => await _stockService.GetCandlesAsync(query, cancellation),

            //    SilverSymbolPrefix => await _imeService.GetCandlesAsync(query, cancellation),

            //    _ => new List<MarketCandle>()
            //};
        }

        private async Task<List<ImeSymbol>> GetImeSymbolsAsync(
            string symbolStart,
            CancellationToken cancellation)
        {
            var imeSymbols = await _unifiedSymbolRepository
                .GetAllAsync(MarketType.Ime, CancellationToken.None);

            var symbols = imeSymbols
                .Where(e => e.SymbolKey.StartsWith(symbolStart))
                .Select(x => x.ToImeSymbol())
                .ToList();

            return symbols
                .OrderBy(x => x.DeliveryDate)
                .ToList();
        }

        private static List<YtmCandle> BuildFutureYtmCandles(
            IEnumerable<MarketCandle> futureCandles,
            IReadOnlyDictionary<string, ImeSymbol> symbolMap)
        {
            var result = new List<YtmCandle>();

            foreach (var candle in futureCandles)
            {
                if (!symbolMap.TryGetValue(candle.InstrumentId, out var symbol))
                    continue;

                result.Add(new YtmCandle
                {
                    InstrumentId = symbol.Symbol,
                    Close = candle.Close,
                    Date = candle.Date,
                    MaturityDate = symbol.DeliveryDate.DayNumber - candle.Date.ToDateOnly().DayNumber
                });
            }

            return result;
        }

        private static List<YtmCandle> BuildSpotYtmCandles(
            IEnumerable<MarketCandle> baseCandles)
        {
            return baseCandles
                .Select(x => new YtmCandle
                {
                    Date = x.Date,
                    Close = x.Close,
                    InstrumentId = x.InstrumentId
                })
                .ToList();
        }

        private static List<CandleResult> BuildSoodChartData(
            IEnumerable<MarketCandle> futures,
            IEnumerable<MarketCandle> spots)
        {
            var futuresMap = BuildDailyCloseMap(futures);
            var spotsMap = BuildDailyCloseMap(spots);

            var commonDates = futuresMap.Keys
                .Intersect(spotsMap.Keys)
                .OrderBy(x => x)
                .ToList();

            var result = new List<CandleResult>();

            decimal runningTotal = 0;
            var hasRunningTotal = false;

            foreach (var date in commonDates)
            {
                var previousDate = date.AddDays(-1);

                if (!futuresMap.TryGetValue(date, out var currentFutureClose) ||
                    !futuresMap.TryGetValue(previousDate, out var previousFutureClose))
                    continue;

                if (!spotsMap.TryGetValue(date, out var currentSpotClose) ||
                    !spotsMap.TryGetValue(previousDate, out var previousSpotClose))
                    continue;

                var futureDelta = previousFutureClose - currentFutureClose;
                var spotDelta = currentSpotClose - previousSpotClose;

                var dailyDifference = spotDelta - futureDelta;

                if (hasRunningTotal)
                    dailyDifference += runningTotal;

                runningTotal = dailyDifference;
                hasRunningTotal = true;

                result.Add(new CandleResult
                {
                    Date = date,
                    PersianDate = DateTimeExtensions.ToPersian(date),
                    Growth = dailyDifference
                });
            }

            return result;
        }

        private static Dictionary<DateOnly, decimal> BuildDailyCloseMap(
            IEnumerable<MarketCandle> candles)
        {
            return candles
                .GroupBy(x => x.Date.ToDateOnly())
                .ToDictionary(
                    g => g.Key,
                    g => g
                        .OrderBy(x => x.Date)
                        .Last()
                        .Close);
        }

        //private static T GetRequiredParameter<T>(
        //    AnalysisRequest request,
        //    string key)
        //{
        //    if (!request.Parameters.TryGetValue(key, out var value) || value is null)
        //        throw new ArgumentException($"Required parameter '{key}' was not provided.");

        //    if (value is T typedValue)
        //        return typedValue;

        //    return (T)Convert.ChangeType(value, typeof(T));
        //}
    }
}


//using MarketAnalyzer.MarketIntelligence.Domain;
//using MarketAnalyzer.MarketIntelligence.Infrastructure;
//using MarketAnalyzer.SharedKernel;

//namespace MarketAnalyzer.MarketIntelligence.Application
//{
//    public class YtmStrategy : IAnalysisStrategy
//    {
//        private readonly IStockService _stockService;
//        private readonly IImeService _imeService;
//        private readonly ICalculateImeContractIndex _calculateImeContractIndex;

//        public string Name => StrategyName.YTM;
//        public YtmStrategy(IStockService stockService,
//            IImeService imeService,
//            ICalculateImeContractIndex calculateImeContractIndex)
//        {
//            _stockService = stockService;
//            _imeService = imeService;
//            _calculateImeContractIndex = calculateImeContractIndex;
//        }

//        public async Task<AnalysisResult> ExecuteAsync(AnalysisRequest request, IAnalysisContext context, CancellationToken cancellation)
//        {
//            // گرفتن پارامترها از request
//            var symbol = request.Parameters["Symbol"].ToString();
//            var calculates = request.Parameters["Calculates"].ToString();
//            var startDate = (DateTime)request.Parameters["StartDate"];
//            var endDate = (DateTime)request.Parameters["EndDate"];

//            var chartData = new List<CandleResult>();
//            if (calculates.Contains("Ytm"))
//            {
//                chartData = await CreateYtm(calculates, symbol, startDate, endDate, cancellation);
//            }
//            else if (calculates.Contains("Sood"))
//            {
//                chartData = await CreateSood(calculates, symbol, startDate, endDate, cancellation);
//            }
//            else
//            {
//                chartData = await CreateIndex(calculates, symbol, startDate, endDate, cancellation);
//            }

//            // برگرداندن نتیجه
//            return new AnalysisResult
//            {
//                Strategy = Name,
//                Data = chartData
//            };
//        }

//        public async Task<List<CandleResult>> CreateIndex(
//            string searchType,
//            string symbolStart,
//            DateTime startDate,
//            DateTime endDate,
//            CancellationToken cancellation)
//        {
//            var future = await GetIndex(searchType, symbolStart, startDate, endDate, cancellation);

//            var baseCandles = await GetBaseCandles(symbolStart, startDate, endDate, cancellation);

//            var ratios = CalculateRatio.Calculate(future, baseCandles, true);

//            return ratios;

//        }

//        public async Task<List<CandleResult>> CreateYtm(
//            string searchType,
//            string symbolStart,
//            DateTime startDate,
//            DateTime endDate,
//            CancellationToken cancellation)
//        {
//            var future = await GetIndex(searchType, symbolStart, startDate, endDate, cancellation);

//            var symbols = await _imeService.GetSymbolsAsync(e => e.Symbol.StartsWith(symbolStart), cancellation);

//            // ترتیب بر اساس دورتی که دورترین سررسید دارد
//            symbols = symbols
//                .OrderByDescending(e => e.DeliveryDate)
//                .ToList();

//            var symbolList = symbols.Select(e => e.Symbol);
//            var ytmCandleFuture = new List<YtmCandle>();

//            foreach (var f in future)
//            {
//                var find = symbols.Find(c => c.Symbol == f.InstrumentId);

//                if (find == null) continue;

//                ytmCandleFuture.Add(new YtmCandle
//                {
//                    InstrumentId = find.Symbol,
//                    Close = f.Close,
//                    Date = f.Date,
//                    MaturityDate = find.DeliveryDate.DayNumber - f.Date.ToDateOnly().DayNumber
//                });
//            }

//            var baseCandles = await GetBaseCandles(symbolStart, startDate, endDate, cancellation);

//            var talaYtmCandle = baseCandles.Select(e => new YtmCandle
//            {
//                Date = e.Date,
//                Close = e.Close,
//                InstrumentId = e.InstrumentId,
//            }).ToList();

//            var ytms = CalculateYTM.Calculate(ytmCandleFuture, talaYtmCandle, true);

//            return ytms;
//        }

//        private async Task<List<MarketCandle>> GetIndex(
//            string searchType,
//            string symbolStart,
//            DateTime startDate,
//            DateTime endDate,
//            CancellationToken cancellation)
//        {
//            var future = new List<MarketCandle>();
//            if (searchType.Contains("Nearest"))
//            {
//                future = await _calculateImeContractIndex.CreateNearestDueDateContractIndex(symbolStart, startDate, endDate, cancellation);
//            }
//            else if (searchType.Contains("Second"))
//            {
//                future = await _calculateImeContractIndex.CreateSecondDueDateContractIndex(symbolStart, startDate, endDate, cancellation);
//            }
//            else if (searchType.Contains("Latest"))
//            {
//                future = await _calculateImeContractIndex.CreateLatestDueDateContractIndex(symbolStart, startDate, endDate, cancellation);
//            }

//            return future;
//        }

//        // سود
//        public async Task<List<CandleResult>> CreateSood(
//            string searchType,
//            string symbolStart,
//            DateTime startDate,
//            DateTime endDate,
//            CancellationToken cancellation)
//        {
//            var future = new List<MarketCandle>();

//            future = await _calculateImeContractIndex.CreateLatestDueDateContractIndex(symbolStart, startDate, endDate, cancellation);

//            var baseCandles = await GetBaseCandles(symbolStart, startDate, endDate, cancellation);

//            var talaCandle = baseCandles.Select(e => new YtmCandle
//            {
//                Date = e.Date,
//                Close = e.Close,
//                InstrumentId = e.InstrumentId,
//            });

//            var chartData = BuildLineChartDataSoodTwoLists(
//                future,
//                baseCandles,
//                "Futures / Spot",
//                true // نمایش به صورت درصد
//            );

//            return chartData;
//        }

//        // سود
//        public List<CandleResult> BuildLineChartDataSoodTwoLists(
//            IEnumerable<MarketCandle> futures,
//            IEnumerable<MarketCandle> spots,
//            string label = "Futures / Spot",
//            bool asPercent = true)
//        {
//            // تبدیل به دیکشنری تاریخ → آخرین قیمت آن روز
//            var futuresMap = futures
//                .GroupBy(c => c.Date.ToDateOnly())
//                .ToDictionary(
//                    g => g.Key,
//                    g => g.OrderBy(x => x.Date).Last().Close);

//            var spotsMap = spots
//                .GroupBy(c => c.Date.ToDateOnly())
//                .ToDictionary(
//                    g => g.Key,
//                    g => g.OrderBy(x => x.Date).Last().Close);

//            // تاریخ‌های مشترک
//            var commonDates = futuresMap.Keys
//                .Intersect(spotsMap.Keys)
//                .OrderBy(d => d)
//                .ToList();

//            var result = new List<CandleResult>();

//            decimal runningTotal = 0;
//            bool hasRunning = false;

//            foreach (var date in commonDates)
//            {
//                var prevDate = date.AddDays(-1);

//                if (!futuresMap.TryGetValue(date, out var currF) ||
//                    !futuresMap.TryGetValue(prevDate, out var prevF))
//                    continue;

//                if (!spotsMap.TryGetValue(date, out var currS) ||
//                    !spotsMap.TryGetValue(prevDate, out var prevS))
//                    continue;

//                var deltaF = prevF - currF;
//                var deltaS = currS - prevS;

//                decimal diff = deltaS - deltaF;

//                if (hasRunning)
//                    diff += runningTotal;

//                runningTotal = diff;
//                hasRunning = true;

//                result.Add(new CandleResult
//                {
//                    Date = date,
//                    PersianDate = DateTimeExtensions.ToPersian(date),
//                    Growth = diff
//                });
//            }

//            return result;
//        }

//        private async Task<List<MarketCandle>> GetBaseCandles(string symbolStart,
//            DateTime startDate,
//            DateTime endDate,
//            CancellationToken cancellation)
//        {
//            // طلا لوتوس
//            // "ETC", "IRTKLOTF0001";
//            // سپرده نقره
//            // "SIL", "BaseIme-Silver";

//            var symbols = await _imeService.GetSymbolsAsync(e => e.Symbol.StartsWith(symbolStart), cancellation);
//            var sym = symbols.OrderBy(e => e.DeliveryDate).FirstOrDefault()?.ParentSymbol ?? string.Empty;
//            var list = new List<MarketCandle>();
//            if (symbolStart == "ETC")
//            {
//                list = await _stockService.GetCandlesAsync(new MarketQueryBuilder()
//               .Between(startDate, endDate)
//               .ForSymbols(sym)
//               .Build(), cancellation);
//            }
//            else if (symbolStart == "SIL")
//            {
//                list = await _imeService.GetCandlesAsync(new MarketQueryBuilder()
//               .Between(startDate, endDate)
//               .ForSymbols(sym)
//               .Build(), cancellation);
//            }
//            return list;
//        }
//    }
//}
