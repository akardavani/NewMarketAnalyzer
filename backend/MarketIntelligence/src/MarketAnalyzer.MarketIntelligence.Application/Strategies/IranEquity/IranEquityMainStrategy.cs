using DocumentFormat.OpenXml.Drawing;
using MarketAnalyzer.MarketIntelligence.Domain;
using MarketAnalyzer.MarketIntelligence.Infrastructure;
using System.Text.Json;

namespace MarketAnalyzer.MarketIntelligence.Application
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
    public sealed class IranEquityMainStrategy : IAnalysisStrategy
    {
        public string Name => StrategyName.IranEquityMain;

        private readonly IUnifiedCandleQueryService _candleQueryService;
        private readonly IUnifiedSymbolRepository _unifiedSymbolRepository;

        public IranEquityMainStrategy(
            IUnifiedCandleQueryService candleQueryService,
            IUnifiedSymbolRepository unifiedSymbolRepository)
        {
            _candleQueryService = candleQueryService;
            _unifiedSymbolRepository = unifiedSymbolRepository;
        }

        public async Task<AnalysisResult> ExecuteAsync(AnalysisRequest request, CancellationToken cancellation)
        {
            // ۱. خواندن داده‌های اکسل (فقط یکبار)
            var excelPath = request.Parameters["InputFilePath"].ToString();
            var excelRows = ExcelReader.Read(excelPath);

            // ۲. لود کردن تمام داده‌های مورد نیاز از دیتابیس (یکبار برای کل بازار)
            var universe = await BuildMarketUniverse(excelRows, cancellation);
            if (universe == null || !universe.Any())
                return new AnalysisResult { Strategy = Name, Data = new { Status = "Empty Universe" } };

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

            return new AnalysisResult
            {
                Strategy = Name,
                Data = new
                {
                    MarketTimestamp = DateTime.Now,
                    TotalSymbolsAnalyzed = rankedStocks.Count,

                    // ۲۰ سهم برتر که هم رتبه بالا دارند و هم کاندید خرید هستند
                    TopPicks = rankedStocks.Where(x => x.IsCandidate).Take(20).ToList(),

                    // وضعیت صنایع برتر
                    LeadingSectors = hotSectors,

                    // برای دیباگ یا نمایش در نمودار
                    FullSectorReport = sectorAnalysis
                }
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
            var allIndices = await _unifiedSymbolRepository
                .GetAllAsync(MarketType.TseIndex, CancellationToken.None);

            var allInstruments = await GetAllSymbols();
            var symbols = allInstruments
                .Select(e=>e.SymbolKey)
                .ToList();

            // مارکت بورس تهران
            // از یکسال پیش تا الان
            // تایم روزانه
            var dailyCandles = await _candleQueryService.GetCandlesAsync(
                new CandleQuery
                {
                    From = DateTime.Today.AddYears(-1),
                    To = DateTime.Today,
                    Timeframe = Timeframe.Daily,
                    SymbolKeys = symbols,
                    Markets = new[] { MarketType.Tse }
                },
                cancellation);

            // مارکت بورس تهران
            // از یکسال پیش تا الان
            // تایم ماهانه
            var monthlyCandles = await _candleQueryService.GetCandlesAsync(
                new CandleQuery
                {
                    From = DateTime.Today.AddYears(-3),
                    To = DateTime.Today,
                    Timeframe = Timeframe.Mountly,
                    SymbolKeys = symbols,
                    Markets = new[] { MarketType.Tse }
                },
                cancellation);

            // مارکت شاخص صنعت
            // از یکسال پیش تا الان
            // تایم روزانه
            var indexCandles = await _candleQueryService.GetCandlesAsync(
                new CandleQuery
                {
                    From = DateTime.Today.AddYears(-1),
                    To = DateTime.Today,
                    Timeframe = Timeframe.Daily,
                    SymbolKeys = symbols,
                    Markets = new[] { MarketType.TseIndex }
                },
                cancellation);            

            var inputs = new List<StockAnalysisInput>();

            foreach (var row in excelRows)
            {
                var instrument = allInstruments
                    .FirstOrDefault(x => x.Symbol == row.Symbol);
                
                if (instrument == null) continue;

                var meta = JsonSerializer.Deserialize<StockInstrumentMetadata>(instrument.MetadataJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var instrumentSectorCode = meta.SectorCode;                

                var industry = allIndices.FirstOrDefault(x => x.Symbol == instrumentSectorCode);
                var marketIndexId = "32097828799138957"; // شاخص کل

                var enriched = new EnrichedStockData
                {
                    ExcelData = row,
                    InstrumentId = instrument.InstrumentId,
                    IndustryId = instrumentSectorCode,
                    IndustryName = industry?.PersianName ?? instrumentSectorCode,

                    DailyCandles = dailyCandles
                                    .Select(e => new MarketCandle(e)).Where(c => c.InstrumentId == instrument.InstrumentId)
                                    .ToList(),
                    MonthlyCandles = monthlyCandles
                                    .Select(e => new MarketCandle(e)).Where(c => c.InstrumentId == instrument.InstrumentId)
                                    .ToList(),
                    IndustryIndexCandles = indexCandles
                                    .Select(e => new MarketCandle(e)).Where(c => c.InstrumentId == industry?.InsCode)
                                    .ToList(),
                    MarketIndexCandles = indexCandles
                                    .Select(e => new MarketCandle(e)).Where(c => c.InstrumentId == marketIndexId)
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

        private async Task<List<UnifiedSymbolRecord>> GetAllSymbols()
        {
            var symbols = await _unifiedSymbolRepository
                .GetAllAsync(MarketType.Tse, CancellationToken.None);

            var symbolKeys = symbols
                .Where(e => e.SymbolKey.EndsWith("0001") &&
                            e.SymbolKey.StartsWith("IRO") &&
                            e.IsActive)                
                .AsEnumerable() // از اینجا به بعد LINQ در حافظه اجرا می‌شود
                .Where(e =>
                {
                    if (string.IsNullOrWhiteSpace(e.MetadataJson))
                        return false;

                    var meta = JsonSerializer.Deserialize<StockInstrumentMetadata>(e.MetadataJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return meta != null
                        //&& !string.IsNullOrEmpty(meta.InstrumentGroupId)
                        //&& meta.InstrumentGroupId.Contains("M1")
                        && meta.SubSectorCode != "6812"
                        && meta.Flow != 4;
                })
                .Distinct()
                .ToList();

            return symbolKeys;
        }
    }

    

}
