using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketData.Application.Reading;
using MarketAnalyzer.MarketIntelligence.Domain;
using MarketAnalyzer.MarketIntelligence.Infrastructure;
using MarketAnalyzer.MarketIntelligence.Application.Ports;
using System.Text.Json;

namespace MarketAnalyzer.MarketIntelligence.Application.Strategies.MultiIndicatorDivergence
{
    public sealed class MultiIndicatorDivergenceHandler
    {
        private readonly IAnalysisCandleReader _candleQueryService;
        private readonly MarketDataIranEquitySymbolCatalog _symbolCatalog;

        public MultiIndicatorDivergenceHandler(
            IAnalysisCandleReader candleQueryService,
            MarketDataIranEquitySymbolCatalog symbolCatalog)
        {
            _candleQueryService = candleQueryService;
            _symbolCatalog = symbolCatalog;
        }

        public async Task<MultiIndicatorDivergenceResponse> HandleAsync(RunMultiIndicatorDivergenceQuery request, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(request);
            if (string.IsNullOrWhiteSpace(request.InputFilePath))
                throw new ArgumentException("An input Excel file path is required.", nameof(request));
            _ = ExcelReader.Read(request.InputFilePath);

            var universe = await BuildMarketUniverse(cancellation);
            if (universe == null || !universe.Any())
                return new MultiIndicatorDivergenceResponse { Status = "Empty Universe" };

            var rawResults = universe.Select(input => ProcessDivergence(input)).ToList();

            // فیلتر و مرتب‌سازی: نمادهایی که حداقل ۲ واگرایی فعال روی آخرین کندل دارند
            var topOpportunities = rawResults
                .Where(x => x.DivergenceCount >= 2)
                .OrderByDescending(x => x.DivergenceCount)
                .ToList();

            return new MultiIndicatorDivergenceResponse
            {
                    MarketTimestamp = DateTime.Now,
                    TotalSymbolsAnalyzed = rawResults.Count,
                    TopPicks = topOpportunities
            };
        }

        private DivergenceStrategyResult ProcessDivergence(StockAnalysisInput input)
        {
            var dailyCandles = CalculationUtils
                .SortAscending(input.Data.DailyCandles)
                .ToList();

            var m = input.Metrics;

            var result = new DivergenceStrategyResult
            {
                Symbol = input.Data.Symbol,
                //LastPrice = dailyCandles.LastOrDefault()?.Close ?? 0,
                //ATR = m.ATR14,
                //FairValue = m.EstimateFairValue,
                //DistanceToSupport = m.DistanceToSupport,
                //DistanceToResistance = m.DistanceToResistance,
                //RawMomentum = m.RelativeStrength30Market,
                //RawLiquidity = m.RealMoneyFlow,
                //RawQuality = m.SectorRotationScore
            };

            if (dailyCandles.Count < 20)
                return result;

            // بررسی ۱۱ اندیکاتور محاسبه شده در لایه MetricBuilder
            CheckAndAddDivergence(result, dailyCandles, m.HistRsi, "RSI");
            CheckAndAddDivergence(result, dailyCandles, m.HistMacdLine, "MACD Line");
            CheckAndAddDivergence(result, dailyCandles, m.HistMacdHistogram, "MACD Hist");
            CheckAndAddDivergence(result, dailyCandles, m.HistStochastic, "Stochastic");
            CheckAndAddDivergence(result, dailyCandles, m.HistCci, "CCI");
            CheckAndAddDivergence(result, dailyCandles, m.HistMomentum, "Momentum");
            CheckAndAddDivergence(result, dailyCandles, m.HistObv, "OBV");
            CheckAndAddDivergence(result, dailyCandles, m.HistVwMacd, "VWMACD");
            CheckAndAddDivergence(result, dailyCandles, m.HistCmf, "CMF");
            CheckAndAddDivergence(result, dailyCandles, m.HistMfi, "MFI");

            // نمونه اندیکاتور خارجی (مثلاً بازدهی کوتاه‌مدت یا تغییرات قیمتی به عنوان اندیکاتور ۱۱ام)
            var priceIndicator = dailyCandles.Select(c => c.Close).ToList();
            CheckAndAddDivergence(result, dailyCandles, priceIndicator, "Extrn");

            return result;
        }

        private void CheckAndAddDivergence(DivergenceStrategyResult result, List<AnalysisCandle> candles, List<decimal> indicator, string name)
        {
            if (indicator == null || indicator.Count != candles.Count) return;

            // پنجره نگاه به عقب (Lookback Range) برای پیدا کردن پیوت‌های قبلی
            int rightStrength = 2; // تعداد کندل‌های سمت راست پیوت
            int leftStrength = 5;  // تعداد کندل‌های سمت چپ پیوت
            int maxLookback = 60;  // حداکثر فاصله برای بررسی واگرایی

            // ۱. بررسی واگرایی مثبت در کف (Regular Bullish Divergence)
            var bullishDiv = FindBullishDivergence(candles, indicator, leftStrength, rightStrength, maxLookback);
            if (bullishDiv != null)
            {
                result.DetectedDivergences.Add(new DivergenceDetection
                {
                    IndicatorName = name,
                    Type = "Bullish (کف)",
                    BarsAgo = bullishDiv.Value
                });
                return;
            }

            // ۲. بررسی واگرایی منفی در سقف (Regular Bearish Divergence)
            var bearishDiv = FindBearishDivergence(candles, indicator, leftStrength, rightStrength, maxLookback);
            if (bearishDiv != null)
            {
                result.DetectedDivergences.Add(new DivergenceDetection
                {
                    IndicatorName = name,
                    Type = "Bearish (سقف)",
                    BarsAgo = bearishDiv.Value
                });
            }
        }

        private int? FindBullishDivergence(List<AnalysisCandle> candles, List<decimal> indicator, int left, int right, int maxLookback)
        {
            int n = candles.Count;
            int currentCandidate = n - 1 - right; // بررسی وقوع پیوت در کندل فعلی (با در نظر گرفتن تأخیر تأیید راست)

            if (!IsPivotLow(candles, currentCandidate, left, right) && !IsPivotLow(indicator, currentCandidate, left, right))
                return null;

            decimal currentPriceLow = candles[currentCandidate].Low;
            decimal currentIndLow = indicator[currentCandidate];

            // جستجو برای پیدا کردن پیوت قبلی در بازه نگاه به عقب
            for (int i = currentCandidate - 1; i >= Math.Max(0, currentCandidate - maxLookback); i--)
            {
                if (IsPivotLow(candles, i, left, right) || IsPivotLow(indicator, i, left, right))
                {
                    decimal prevPriceLow = candles[i].Low;
                    decimal prevIndLow = indicator[i];

                    // ساختار کلاسیک واگرایی مثبت: قیمت کف پایین‌تر ساخته اما اندیکاتور کف بالاتر ساخته
                    if (currentPriceLow < prevPriceLow && currentIndLow > prevIndLow)
                    {
                        return (n - 1 - currentCandidate); // بازگرداندن تعداد کندل گذشته
                    }
                }
            }

            return null;
        }

        private int? FindBearishDivergence(List<AnalysisCandle> candles, List<decimal> indicator, int left, int right, int maxLookback)
        {
            int n = candles.Count;
            int currentCandidate = n - 1 - right;

            if (!IsPivotHigh(candles, currentCandidate, left, right) && !IsPivotHigh(indicator, currentCandidate, left, right))
                return null;

            decimal currentPriceHigh = candles[currentCandidate].High;
            decimal currentIndHigh = indicator[currentCandidate];

            for (int i = currentCandidate - 1; i >= Math.Max(0, currentCandidate - maxLookback); i--)
            {
                if (IsPivotHigh(candles, i, left, right) || IsPivotHigh(indicator, i, left, right))
                {
                    decimal prevPriceHigh = candles[i].High;
                    decimal prevIndHigh = indicator[i];

                    // ساختار کلاسیک واگرایی منفی: قیمت سقف بالاتر ساخته اما اندیکاتور سقف پایین‌تر ساخته
                    if (currentPriceHigh > prevPriceHigh && currentIndHigh < prevIndHigh)
                    {
                        return (n - 1 - currentCandidate);
                    }
                }
            }

            return null;
        }

        // متدهای شناسایی پیوت‌ها روی لیست‌های قیمت و اندیکاتور
        private bool IsPivotLow(List<AnalysisCandle> source, int index, int left, int right)
        {
            if (index - left < 0 || index + right >= source.Count) return false;
            decimal val = source[index].Low;
            for (int i = index - left; i <= index + right; i++)
            {
                if (i == index) continue;
                if (source[i].Low < val) return false;
            }
            return true;
        }

        private bool IsPivotLow(List<decimal> source, int index, int left, int right)
        {
            if (index - left < 0 || index + right >= source.Count) return false;
            decimal val = source[index];
            for (int i = index - left; i <= index + right; i++)
            {
                if (i == index) continue;
                if (source[i] < val) return false;
            }
            return true;
        }

        private bool IsPivotHigh(List<AnalysisCandle> source, int index, int left, int right)
        {
            if (index - left < 0 || index + right >= source.Count) return false;
            decimal val = source[index].High;
            for (int i = index - left; i <= index + right; i++)
            {
                if (i == index) continue;
                if (source[i].High > val) return false;
            }
            return true;
        }

        private bool IsPivotHigh(List<decimal> source, int index, int left, int right)
        {
            if (index - left < 0 || index + right >= source.Count) return false;
            decimal val = source[index];
            for (int i = index - left; i <= index + right; i++)
            {
                if (i == index) continue;
                if (source[i] > val) return false;
            }
            return true;
        }

        // کدهای مربوط به واکشی دیتای Universe از متد قبلی شما
        private async Task<List<StockAnalysisInput>> BuildMarketUniverse(CancellationToken cancellation)
        {
            var allIndices = await _symbolCatalog.GetTseIndicesAsync(cancellation);
            var allInstruments = await GetAllSymbols(cancellation);
            var symbols = allInstruments.Select(e => e.SymbolKey).ToList();

            var dailyCandles = await _candleQueryService.GetCandlesAsync(new MultiSymbolCandleQuery
            {
                From = DateTime.Today.AddYears(-1),
                To = DateTime.Today,
                Timeframe = Timeframe.Daily,
                SymbolKeys = symbols,
                Markets = new[] { "tse" }
            }, cancellation);

            var monthlyCandles = await _candleQueryService.GetCandlesAsync(new MultiSymbolCandleQuery
            {
                From = DateTime.Today.AddYears(-3),
                To = DateTime.Today,
                Timeframe = Timeframe.Mountly,
                SymbolKeys = symbols,
                Markets = new[] { "tse" }
            }, cancellation);

            var indexCandles = await _candleQueryService.GetCandlesAsync(new MultiSymbolCandleQuery
            {
                From = DateTime.Today.AddYears(-1),
                To = DateTime.Today,
                Timeframe = Timeframe.Daily,
                SymbolKeys = symbols,
                Markets = new[] { "tse-index" }
            }, cancellation);

            var inputs = new List<StockAnalysisInput>();

            foreach (var instrument in allInstruments)
            {
                //var instrument = allInstruments.FirstOrDefault(x => x.Symbol == row.Symbol);
                //if (instrument == null) continue;

                var meta = JsonSerializer.Deserialize<TseStockMetadata>(instrument.MetadataJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var instrumentSectorCode = meta.SectorCode;
                var industry = allIndices.FirstOrDefault(x => x.SymbolKey == instrumentSectorCode);
                var marketIndexId = "32097828799138957";

                var enriched = new EnrichedStockData
                {
                    //ExcelData = row,
                    Symbol = instrument.SymbolKey,
                    InstrumentId = instrument.SymbolKey,
                    IndustryId = instrumentSectorCode,
                    IndustryName = industry?.PersianName ?? instrumentSectorCode,
                    DailyCandles = dailyCandles.Where(c => c.InstrumentId == instrument.SymbolKey).ToList(),
                    MonthlyCandles = monthlyCandles.Where(c => c.InstrumentId == instrument.SymbolKey).ToList(),
                    IndustryIndexCandles = indexCandles.Where(c => c.InstrumentId == industry?.SymbolKey).ToList(),
                    MarketIndexCandles = indexCandles.Where(c => c.InstrumentId == marketIndexId).ToList()
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

    public class DivergenceDetection
    {
        
        public string IndicatorName { get; set; }
        public string Type { get; set; } // Bullish (کف) یا Bearish (سقف)
        public int BarsAgo { get; set; }
    }

    public class DivergenceStrategyResult //: IranEquityStrategyResult
    {
        public string Symbol { get; set; }
        public List<DivergenceDetection> DetectedDivergences { get; set; } = new();
        public int DivergenceCount => DetectedDivergences.Count;
        public string MarketPosition => DetectedDivergences.Any(x => x.Type.Contains("Bullish")) ? "کف (Potentially Bottom)" :
                                       DetectedDivergences.Any(x => x.Type.Contains("Bearish")) ? "سقف (Potentially Top)" : "خنثی";
    }
}
