using MarketAnalyzer.MarketData.Domain;
using System.Text.Json;

namespace MarketAnalyzer.MarketIntelligence.Application.Strategies.IranEquityMain
{
    /// <summary>
    /// استراتژی جامع کوانت و رتبه‌بندی چند‌فاکتوری بازار سهام ایران (Iran Equity Quant Strategy)
    /// 
    /// این موتور تحلیلی با استفاده از مدل رتبه‌بندی متقاطع (Cross-Sectional Ranking) و متدولوژی صندوق‌های پوشش ریسک (Hedge Funds)، 
    /// کل بازار را در سه لایه تحلیل می‌کند:
    /// 
    /// ۱. لایه تحلیل نماد (Individual Stock Analysis):
    ///    - محاسبه متریک‌های تکنیکال پیشرفته (ATR, Distance to Support/Resistance).
    ///    - استخراج فاکتورهای خام: Momentum (ترکیب RS در بازه‌های زمانی مختلف)، Liquidity (ورود پول هوشمند و قدرت حجم).
    ///    - تعیین نقاط ورود، حد ضرر و حد سود داینامیک بر اساس نوسانات واقعی هر سهم (ATR-Based).
    /// 
    /// ۲. لایه رتبه‌بندی کوانت (Quant Ranking Engine):
    ///    - نرمال‌سازی فاکتورها با استفاده از Z-Score: سنجش قدرت هر نماد نسبت به میانگین کل بازار.
    ///    - تخصیص امتیاز نهایی (Final Score) و رتبه درصدی (Percentile): شناسایی ۲۰٪ برتر بازار (Tier A/A+).
    /// 
    /// ۳. لایه تحلیل صنعت (Sector Analysis):
    ///    - تجمیع نتایج نمادها در گروه‌های صنعتی برای شناسایی "چرخش صنایع" (Sector Rotation).
    ///    - شناسایی صنایع پیشرو (Leading Sectors) که بیشترین تمرکز نمادهای رتبه A را دارند.
    /// 
    /// خروجی نهایی:
    /// لیستی از Top Picks (بهترین فرصت‌های خرید) که تاییدیه ورود پول، قدرت روند و نسبت ریسک به ریوارد مناسب دارند، 
    /// به همراه گزارش کامل وضعیت صنایع برای تصمیم‌گیری استراتژیک.
    /// </summary>
    public sealed class IranEquityMainHandler
    {

        private readonly IAnalysisCandleReader _candleQueryService;
        private readonly MarketDataIranEquitySymbolCatalog _symbolCatalog;

        public IranEquityMainHandler(
            IAnalysisCandleReader candleQueryService,
            MarketDataIranEquitySymbolCatalog symbolCatalog)
        {
            _candleQueryService = candleQueryService;
            _symbolCatalog = symbolCatalog;
        }

        public async Task<IranEquityMainResponse> HandleAsync(RunIranEquityMainQuery request, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(request);
            if (string.IsNullOrWhiteSpace(request.InputFilePath))
                throw new ArgumentException("An input Excel file path is required.", nameof(request));

            // ۱. خواندن داده‌های اکسل (فقط یکبار)
            var excelRows = ExcelReader.Read(request.InputFilePath);

            // ۲. لود کردن تمام داده‌های مورد نیاز از دیتابیس (یکبار برای کل بازار)
            var universe = await BuildMarketUniverse(excelRows, cancellation);
            if (universe == null || !universe.Any())
                return new IranEquityMainResponse { Status = "Empty Universe" };

            // پردازش تک‌تک نمادها (محاسبه ATR, Support, Raw Factors)
            var rawResults = universe.Select(input => ProcessSingleStock(input)).ToList();

            // ۳. رتبه‌بندی (اعمال Z-Score و محاسبه FinalScore و Percentile)
            var rankedStocks = RankingEngine.RankUniverse(rawResults);

            // ۴. تحلیل صنایع (تجمیع نتایج بر اساس IndustryName)
            var sectorAnalysis = RankingEngine.AnalyzeSectors(rankedStocks, universe);

            // ۵. فیلتر صنایع داغ
            var hotSectors = sectorAnalysis
                .Where(s => s.Status.Contains("Hot") || s.TopTierCount >= 2)
                .OrderByDescending(s => s.AverageScore)
                .ToList();

            return new IranEquityMainResponse
            {
                    MarketTimestamp = DateTime.Now,
                    TotalSymbolsAnalyzed = rankedStocks.Count,

                    // ۲۰ سهم برتر که هم رتبه بالا دارند و هم کاندید خرید هستند
                    TopPicks = rankedStocks.Where(x => x.IsCandidate).Take(20).ToList(),

                    // وضعیت صنایع برتر
                    LeadingSectors = hotSectors,

                    // برای دیباگ یا نمایش در نمودار
                    FullSectorReport = sectorAnalysis
            };
        }

        private IranEquityStrategyResult ProcessSingleStock(StockAnalysisInput input)
        {
            var m = input.Metrics;
            var res = new IranEquityStrategyResult
            {
                Symbol = input.Data.ExcelData.Symbol,
                LastPrice = input.Data.ExcelData.LastPrice,
                ATR = m.ATR14,
                FairValue = m.EstimateFairValue,
                DistanceToSupport = m.DistanceToSupport,
                DistanceToResistance = m.DistanceToResistance,

                // مقادیر خام برای موتور رتبه‌بندی
                RawMomentum = (m.RelativeStrength30Market * 0.5m) + (m.RelativeStrength60Market * 0.5m),
                RawLiquidity = m.RealMoneyFlow + (m.VolumeExpansionScore * 2),
                RawQuality = m.SectorRotationScore + m.ZScoreIndustry,

                // پر کردن دیکشنری‌ها برای شفافیت خروجی
                FactorScores = new Dictionary<string, decimal> {
                    { "Momentum", 0 }, // بعد از رتبه‌بندی پر می‌شود
                    { "Liquidity", 0 },
                    { "Quality", 0 }
                },
                FeatureValues = new Dictionary<string, decimal> {
                    { "RS_Percentile", m.RelativeStrengthPercentile },
                    { "MoneyFlow", m.RealMoneyFlow },
                    { "VolScore", m.VolumeExpansionScore }
                }
            };

            // محاسبه سیگنال داینامیک
            var signal = BuildAdaptiveSignal(res);
            res.Entry = signal.Entry;
            res.StopLoss = signal.StopLoss;
            res.TakeProfits = signal.TakeProfits;

            return res;
        }

        private static SignalResult BuildAdaptiveSignal(IranEquityStrategyResult res)
        {
            // ۱. محاسبه حد ضرر (Stop Loss)
            // حد ضرر را بین دو مقدار مقایسه می‌کنیم: 
            // الف) ۲ برابر ATR زیر قیمت (خروج بر اساس نوسان)
            // ب) ۵ درصد زیر نزدیک‌ترین حمایت (خروج بر اساس ساختار)
            decimal atrBasedSl = res.LastPrice - (res.ATR * 2.0m);
            decimal supportBasedSl = res.DistanceToSupport.HasValue
                ? res.LastPrice * (1 - (res.DistanceToSupport.Value + 0.05m))
                : res.LastPrice * 0.93m; // اگر حمایتی یافت نشد، ۷٪ ثابت

            decimal finalSl = Math.Max(atrBasedSl, supportBasedSl);

            // ۲. محاسبه نقطه ورود (Entry)
            // معمولاً کمی بالاتر از قیمت فعلی برای تایید (مثلاً ۰.۵ درصد)
            decimal entryPrice = res.LastPrice * 1.005m;

            // ۳. محاسبه حد سودها (Take Profits)
            // TP1: در نزدیکی اولین مقاومت یا نسبت ۱ به ۱ ریسک به ریوارد
            // TP2: نسبت ۱ به ۲ ریسک به ریوارد یا مقاومت دورتر
            decimal riskAmount = entryPrice - finalSl;

            decimal tp1 = res.DistanceToResistance.HasValue && res.DistanceToResistance > 0.03m
                ? res.LastPrice * (1 + (res.DistanceToResistance.Value - 0.01m)) // یک درصد قبل از مقاومت
                : entryPrice + (riskAmount * 1.5m);

            decimal tp2 = tp1 + (riskAmount * 1.5m);

            return new SignalResult
            {
                Entry = entryPrice,
                StopLoss = finalSl,
                TakeProfits = new[] { Math.Round(tp1, 0), Math.Round(tp2, 0) }
            };
        }

        private async Task<List<StockAnalysisInput>> BuildMarketUniverse(List<ExcelStockRow> excelRows, CancellationToken cancellation)
        {
            var allIndices = await _symbolCatalog.GetTseIndicesAsync(cancellation);

            var allInstruments = await GetAllSymbols(cancellation);
            var symbols = allInstruments
                .Select(e=>e.SymbolKey)
                .ToList();

            // مارکت بورس تهران
            // از یکسال پیش تا الان
            // تایم روزانه
            var dailyCandles = await _candleQueryService.GetCandlesAsync(
                new MultiSymbolCandleQuery
                {
                    From = DateTime.Today.AddYears(-1),
                    To = DateTime.Today,
                    Timeframe = Timeframe.Daily,
                    SymbolKeys = symbols,
                    Markets = new[] { "tse" }
                },
                cancellation);

            // مارکت بورس تهران
            // از یکسال پیش تا الان
            // تایم ماهانه
            var monthlyCandles = await _candleQueryService.GetCandlesAsync(
                new MultiSymbolCandleQuery
                {
                    From = DateTime.Today.AddYears(-3),
                    To = DateTime.Today,
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
                    From = DateTime.Today.AddYears(-1),
                    To = DateTime.Today,
                    Timeframe = Timeframe.Daily,
                    SymbolKeys = symbols,
                    Markets = new[] { "tse-index" }
                },
                cancellation);            

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

        private Task<List<SymbolRef>> GetAllSymbols(CancellationToken cancellationToken)
            => _symbolCatalog.GetActiveTseStocksAsync(cancellationToken);
    }
}
