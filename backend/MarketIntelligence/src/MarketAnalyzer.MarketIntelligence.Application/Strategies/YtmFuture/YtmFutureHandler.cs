using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketData.Application.Reading;
using MarketAnalyzer.MarketIntelligence.Domain;
using MarketAnalyzer.MarketIntelligence.Application.Ports;
using MarketAnalyzer.MarketIntelligence.Infrastructure;
using MarketAnalyzer.SharedKernel;

namespace MarketAnalyzer.MarketIntelligence.Application.Strategies.YtmFuture
{
    public sealed class YtmFutureHandler
    {

        // طلا لوتوس
        // "ETC", "IRTKLOTF0001";
        // سپرده نقره
        // "SIL", "BaseIme-Silver";

        private readonly IAnalysisCandleReader _candleQueryService;
        private readonly IMarketCatalogReader _catalogReader;
        private readonly ICalculateImeContractIndex _calculateImeContractIndex;

        public YtmFutureHandler(
            IAnalysisCandleReader candleQueryService,
             IMarketCatalogReader catalogReader,
            ICalculateImeContractIndex calculateImeContractIndex)
        {
            _candleQueryService = candleQueryService;
            _catalogReader = catalogReader;
            _calculateImeContractIndex = calculateImeContractIndex;
        }

        public async Task<IReadOnlyList<CandleResult>> HandleAsync(
            RunYtmFutureQuery request,
            CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(request);
            if (string.IsNullOrWhiteSpace(request.Symbol) || string.IsNullOrWhiteSpace(request.Calculates))
                throw new ArgumentException("Symbol and calculation type are required.", nameof(request));
            if (request.StartDate > request.EndDate)
                throw new ArgumentException("Start date must be earlier than or equal to end date.", nameof(request));

            return await CreateChartDataAsync(request.Calculates, request.Symbol, request.StartDate, request.EndDate, cancellation);
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

        private async Task<List<AnalysisCandle>> GetIndexCandlesAsync(
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

            return new List<AnalysisCandle>();
        }

        private async Task<List<AnalysisCandle>> GetBaseCandlesAsync(
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
                return new List<AnalysisCandle>();

            var query = new MarketQueryBuilder()
                .Between(startDate, endDate)
                .ForSymbols(parentSymbol)
                .Build();

            var baseSymbol = await _candleQueryService.GetCandlesAsync(
                new MultiSymbolCandleQuery
                {
                    From = startDate,
                    To = endDate,
                    Timeframe = Timeframe.Daily,
                    SymbolKeys = new List<string> { parentSymbol },
                    Markets = new[]
                    {
                        "tse",
                        "ime"
                    }
                },
                cancellation);

            return baseSymbol
                .Select(e => e)
                .ToList();

            //return symbolStart switch
            //{
            //    EtcSymbolPrefix => await _stockService.GetCandlesAsync(query, cancellation),

            //    SilverSymbolPrefix => await _imeService.GetCandlesAsync(query, cancellation),

            //    _ => new List<AnalysisCandle>()
            //};
        }

        private async Task<List<ImeSymbol>> GetImeSymbolsAsync(
            string symbolStart,
            CancellationToken cancellation)
        {
            var imeSymbols = await _catalogReader.SearchAllAsync(
                new SymbolSearchQuery { MarketKey = "ime", SymbolKeyPrefix = symbolStart, OnlyActive = false },
                cancellation);

            var symbols = imeSymbols
                .Select(x => x.ToImeAnalysisSymbol())
                .ToList();

            return symbols
                .OrderBy(x => x.DeliveryDate)
                .ToList();
        }

        private static List<YtmCandle> BuildFutureYtmCandles(
            IEnumerable<AnalysisCandle> futureCandles,
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
            IEnumerable<AnalysisCandle> baseCandles)
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
            IEnumerable<AnalysisCandle> futures,
            IEnumerable<AnalysisCandle> spots)
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
            IEnumerable<AnalysisCandle> candles)
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
