using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketData.Application.Reading;
using MarketAnalyzer.MarketIntelligence.Domain;
using MarketAnalyzer.MarketIntelligence.Infrastructure;
using MarketAnalyzer.MarketIntelligence.Application.Ports;
using MarketAnalyzer.SharedKernel;
using System.Text.Json;

namespace MarketAnalyzer.MarketIntelligence.Application.Strategies.BottomReversal
{
    public sealed class BottomReversalHandler
    {
        private readonly IAnalysisCandleReader _candleQueryService;
        private readonly MarketDataIranEquitySymbolCatalog _symbolCatalog;

        public BottomReversalHandler(
            IAnalysisCandleReader candleQueryService,
            MarketDataIranEquitySymbolCatalog symbolCatalog)
        {
            _candleQueryService = candleQueryService;
            _symbolCatalog = symbolCatalog;
        }

        public async Task<BottomReversalResponse> HandleAsync(
            RunBottomReversalQuery request,
            CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(request);
            if (string.IsNullOrWhiteSpace(request.InputFilePath))
                throw new ArgumentException("An input Excel file path is required.", nameof(request));
            if (request.StartDate > request.EndDate)
                throw new ArgumentException("Start date must be earlier than or equal to end date.", nameof(request));

            var excelRows = ExcelReader.Read(request.InputFilePath);

            var universe = await BuildMarketUniverse(excelRows, request, cancellation);

            if (universe == null || !universe.Any())
            {
                return new BottomReversalResponse
                {
                    Status = "Empty Universe"
                };
            }

            var rawBottomResults = universe
                .Select(input => ProcessBottomReversal(input))
                .ToList();

            var rankedBase = RankingEngine.RankUniverse(
                rawBottomResults.Cast<IranEquityStrategyResult>().ToList()
            );

            var rankedStocks = rankedBase
                .Cast<BottomReversalResult>()
                .ToList();

            var sectorAnalysis = RankingEngine.AnalyzeSectors(
                rankedStocks.Cast<IranEquityStrategyResult>().ToList(),
                universe
            );

            var hotSectors = sectorAnalysis
                .Where(s => s.Status.Contains("Hot") || s.TopTierCount >= 2)
                .OrderByDescending(s => s.AverageScore)
                .ToList();

            return new BottomReversalResponse
            {
                MarketTimestamp = DateTime.Now,
                TotalSymbolsAnalyzed = rankedStocks.Count,
                TopPicks = rankedStocks.Where(x => x.IsCandidate).Take(20).ToList(),
                LeadingSectors = hotSectors,
                FullSectorReport = sectorAnalysis
            };
        }

        private async Task<List<StockAnalysisInput>> BuildMarketUniverse(List<ExcelStockRow> excelRows, RunBottomReversalQuery request, CancellationToken cancellation)
        {
            var allIndices = await _symbolCatalog.GetTseIndicesAsync(cancellation);

            var allInstruments = await GetAllSymbols(cancellation);
            var symbols = allInstruments
                .Select(e => e.SymbolKey)
                .ToList();

            // مارکت بورس تهران
            // از یکسال پیش تا الان
            // تایم روزانه
            var dailyCandles = await _candleQueryService.GetCandlesAsync(
                new MultiSymbolCandleQuery
                {
                    From = request.StartDate.AddYears(-1),
                    To = request.EndDate,
                    Timeframe = Timeframe.Daily,
                    Markets = new[] { "tse" }
                },
                cancellation);

            // مارکت بورس تهران
            // از یکسال پیش تا الان
            // تایم ماهانه
            var monthlyCandles = await _candleQueryService.GetCandlesAsync(
                new MultiSymbolCandleQuery
                {
                    From = request.StartDate.AddYears(-3),
                    To = request.EndDate,
                    Timeframe = Timeframe.Mountly,
                    SymbolKeys = symbols,
                    Markets = new[] { "tse" }
                },
                cancellation);

            // مارکت شاخص صنعت
            // از یکسال پیش تا الان
            // تایم روزانه
            var indexCandles = await _candleQueryService.GetCandlesAsync(
                new MultiSymbolCandleQuery
                {
                    From = request.StartDate.AddYears(-1),
                    To = request.EndDate,
                    Timeframe = Timeframe.Daily,
                    Markets = new[] { "tse-index" }
                },
                cancellation);

            //var allInstruments = await _stockService.GetSymbolsAsync(e => e.Valid == 1, cancellation);
            //var allIndices = await _tseTmcIndexService.GetSymbolsAsync(null, cancellation);

            //var dailyCandles = await _stockService.GetCandlesAsync(new MarketQueryBuilder()
            //    .Between(request.StartDate.AddYears(-1), request.EndDate)
            //    .WithTimeframe(TimeframeDto.Daily).Build(), cancellation);

            //var monthlyCandles = await _stockService.GetCandlesAsync(new MarketQueryBuilder()
            //    .Between(request.StartDate.AddYears(-3), request.EndDate)
            //    .WithTimeframe(TimeframeDto.Mountly).Build(), cancellation);

            //var indexCandles = await _tseTmcIndexService.GetCandlesAsync(new MarketQueryBuilder()
            //    .Between(request.StartDate.AddYears(-1), request.EndDate)
            //    .WithTimeframe(TimeframeDto.Daily).Build(), cancellation);

            var inputs = new List<StockAnalysisInput>();

            foreach (var row in excelRows)
            {
                var instrument = allInstruments
                    .FirstOrDefault(x => x.PersianName == row.Symbol || x.SymbolKey == row.Symbol);

                if (instrument == null) continue;

                var meta = JsonSerializer.Deserialize<TseStockMetadata>(instrument.MetadataJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var instrumentSectorCode = meta.SectorCode;

                var industry = allIndices.FirstOrDefault(x => x.SymbolKey == instrumentSectorCode);
                var marketIndexId = "32097828799138957"; // شاخص کل

                var enriched = new EnrichedStockData
                {
                    ExcelData = row,
                    InstrumentId = instrument.SymbolKey,
                    IndustryId = instrumentSectorCode,
                    IndustryName = industry?.PersianName ?? instrumentSectorCode,

                    DailyCandles = dailyCandles
                                    .Where(c => c.InstrumentId == instrument.SymbolKey)
                                    .ToList(),
                    MonthlyCandles = monthlyCandles
                                    .Where(c => c.InstrumentId == instrument.SymbolKey)
                                    .ToList(),
                    IndustryIndexCandles = indexCandles
                                    .Where(c => c.InstrumentId == industry?.SymbolKey)
                                    .ToList(),
                    MarketIndexCandles = indexCandles
                                    .Where(c => c.InstrumentId == marketIndexId)
                                    .ToList()
                };

                inputs.Add(new StockAnalysisInput
                {
                    Data = enriched,
                    Metrics = MetricBuilderEngine.Build(enriched)
                });
            }
            return inputs;
        }
        private BottomReversalResult ProcessBottomReversal(StockAnalysisInput input)
        {
            var m = input.Metrics;
            var data = input.Data.ExcelData;

            /*
             * فاکتور ۱: اشباع فروش
             * هرچه RSI و MFI پایین‌تر باشند، امتیاز بیشتر می‌شود.
             * مثال:
             * RSI = 25 => 75 امتیاز خام
             * MFI = 30 => 70 امتیاز خام
             */
            decimal rawOversold =
                ((100m - m.RSI14) * 0.60m) +
                ((100m - m.MFI14) * 0.40m);

            /*
             * فاکتور ۲: نزدیکی به کف
             * فرض این است که DistanceTo60DayLow و DistanceTo120DayLow درصدی هستند.
             * اگر مقدارشان مثلا 5 باشد یعنی ۵ درصد بالاتر از کف.
             */
            decimal rawProximity =
                ((100m - m.DistanceTo60DayLow) * 0.60m) +
                ((100m - m.DistanceTo120DayLow) * 0.40m);

            /*
             * فاکتور ۳: قدرت برگشت
             * ReversalCandleScore معمولا بهتر است ۰ تا ۱۰۰ باشد.
             * VolumeRatio20 مثلا اگر ۲ باشد یعنی حجم دو برابر میانگین ۲۰ روزه.
             */
            decimal rawReversalStrength =
                (m.ReversalCandleScore * 0.60m) +
                (Math.Min(m.VolumeRatio20, 3m) / 3m * 100m * 0.40m);

            /*
             * فاکتور ۴: کیفیت زمینه‌ای
             * این فاکتور برای این است که سهمی که از کف برمی‌گردد،
             * در صنعت کاملاً مرده و بدون ورود پول نباشد.
             */
            decimal rawBottomQuality =
                (m.SectorRotationScore * 0.60m) +
                (m.ZScoreIndustry * 0.40m);

            var res = new BottomReversalResult
            {
                Symbol = data.Symbol,
                LastPrice = data.LastPrice,

                ATR = m.ATR14,
                FairValue = m.EstimateFairValue,
                DistanceToSupport = m.DistanceToSupport,
                DistanceToResistance = m.DistanceToResistance,

                RawOversold = rawOversold,
                RawProximity = rawProximity,
                RawReversalStrength = rawReversalStrength,
                RawBottomQuality = rawBottomQuality,

                /*
                 * مپ کردن فاکتورهای برگشت از کف به فیلدهای عمومی RankingEngine
                 */
                RawMomentum = rawReversalStrength,
                RawLiquidity = rawProximity,
                RawQuality = rawBottomQuality,

                RawFactors = new Dictionary<string, decimal>
                {
                    { "Oversold", Math.Round(rawOversold, 2) },
                    { "Proximity", Math.Round(rawProximity, 2) },
                    { "ReversalStrength", Math.Round(rawReversalStrength, 2) },
                    { "BottomQuality", Math.Round(rawBottomQuality, 2) }
                },

                FactorScores = new Dictionary<string, decimal>
                {
                    { "Momentum", 0 },
                    { "Liquidity", 0 },
                    { "Quality", 0 }
                },

                FeatureValues = new Dictionary<string, decimal>
                {
                    { "RSI14", Math.Round(m.RSI14, 2) },
                    { "MFI14", Math.Round(m.MFI14, 2) },
                    { "DistanceTo60DayLow", Math.Round(m.DistanceTo60DayLow, 2) },
                    { "DistanceTo120DayLow", Math.Round(m.DistanceTo120DayLow, 2) },
                    { "ReversalCandleScore", Math.Round(m.ReversalCandleScore, 2) },
                    { "VolumeRatio20", Math.Round(m.VolumeRatio20, 2) },
                    { "SectorRotationScore", Math.Round(m.SectorRotationScore, 2) },
                    { "ZScoreIndustry", Math.Round(m.ZScoreIndustry, 2) }
                },

                Metrics = new Dictionary<string, decimal>
                {
                    { "ATR14", Math.Round(m.ATR14, 2) },
                    { "DistanceToSupport", Math.Round(m.DistanceToSupport ?? 0, 2) },
                    { "DistanceToResistance", Math.Round(m.DistanceToResistance ?? 0, 2) }
                }};

            AddBottomReversalReasons(res, m);

            var signal = BuildAdaptiveSignal(res);
            res.Entry = signal.Entry;
            res.StopLoss = signal.StopLoss;
            res.TakeProfits = signal.TakeProfits;

            return res;
        }

        private static SignalResult BuildAdaptiveSignal(BottomReversalResult res)
        {
            // ۱. محاسبه حد ضرر (Stop Loss) بر اساس نوسان و حمایت
            // الف) ۲ برابر ATR زیر قیمت (خروج بر اساس نوسانات سهم)
            decimal atrBasedSl = res.LastPrice - (res.ATR * 2.0m);

            // ب) ۵ درصد زیر نزدیک‌ترین حمایت (خروج بر اساس ساختار قیمت)
            decimal supportBasedSl = res.DistanceToSupport.HasValue && res.DistanceToSupport.Value > 0
                ? res.LastPrice * (1 - (res.DistanceToSupport.Value + 0.05m))
                : res.LastPrice * 0.93m; // اگر حمایتی یافت نشد، ۷٪ ثابت به عنوان پیش‌فرض

            // انتخاب حد ضرر ایمن‌تر (عددی که به قیمت فعلی دورتر است معمولا برای کف‌خواری بهتر است)
            decimal finalSl = Math.Min(atrBasedSl, supportBasedSl);

            // ۲. محاسبه نقطه ورود (Entry)
            // برای تایید برگشت، معمولاً ورود را ۰.۵ تا ۱ درصد بالاتر از قیمت فعلی قرار می‌دهیم
            decimal entryPrice = res.LastPrice * 1.01m;

            // ۳. محاسبه حد سودها (Take Profits) بر اساس ریسک به ریوارد
            decimal riskAmount = entryPrice - finalSl;
            if (riskAmount <= 0) riskAmount = entryPrice * 0.05m; // مقدار حداقلی برای جلوگیری از خطای محاسباتی

            // TP1: در نزدیکی اولین مقاومت یا نسبت ۱.۵ به ۱ ریسک به ریوارد
            decimal tp1 = res.DistanceToResistance.HasValue && res.DistanceToResistance > 0.03m
                ? res.LastPrice * (1 + (res.DistanceToResistance.Value - 0.01m)) // یک درصد قبل از مقاومت
                : entryPrice + (riskAmount * 1.5m);

            // TP2: نسبت ۳ به ۱ ریسک به ریوارد برای گرفتن موج اصلی برگشت
            decimal tp2 = entryPrice + (riskAmount * 3.0m);

            return new SignalResult
            {
                Entry = Math.Round(entryPrice, 0),
                StopLoss = Math.Round(finalSl, 0),
                TakeProfits = new[] { Math.Round(tp1, 0), Math.Round(tp2, 0) }
            };
        }

        private Task<List<SymbolRef>> GetAllSymbols(CancellationToken cancellationToken)
            => _symbolCatalog.GetActiveTseStocksAsync(cancellationToken);
        private static void AddBottomReversalReasons(BottomReversalResult res, MarketDerivedMetrics m)
        {
            if (m.RSI14 <= 30)
                res.Reasons.Add("RSI زیر ۳۰؛ سهم در محدوده اشباع فروش است.");

            if (m.MFI14 <= 30)
                res.Reasons.Add("MFI زیر ۳۰؛ خروج یا ضعف شدید جریان پول قبلی مشاهده می‌شود.");

            if (m.DistanceTo60DayLow <= 5)
                res.Reasons.Add("قیمت کمتر از ۵٪ با کف ۶۰ روزه فاصله دارد.");

            if (m.DistanceTo120DayLow <= 8)
                res.Reasons.Add("قیمت نزدیک کف ۱۲۰ روزه است.");

            if (m.ReversalCandleScore >= 60)
                res.Reasons.Add("کندل برگشتی قابل قبول مشاهده شده است.");

            if (m.VolumeRatio20 >= 1.5m)
                res.Reasons.Add("حجم معاملات بالاتر از میانگین ۲۰ روزه است.");

            if (m.SectorRotationScore > 0)
                res.Reasons.Add("صنعت سهم نشانه‌هایی از چرخش مثبت دارد.");
        }
    }
}
