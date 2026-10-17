using MarketAnalyzer.Domain;
using MarketAnalyzer.Analysis.Infrastructure;
using MarketAnalyzer.SharedKernel;

namespace MarketAnalyzer.AnalysisStrategy
{
    public class CalculateImeContractIndex : ICalculateImeContractIndex
    {
        private readonly IUnifiedSymbolRepository _unifiedSymbolRepository;
        private readonly IUnifiedCandleQueryService _candleQueryService;

        public CalculateImeContractIndex(IUnifiedSymbolRepository unifiedSymbolRepository, IUnifiedCandleQueryService candleQueryService)
        {
            _unifiedSymbolRepository = unifiedSymbolRepository;
            _candleQueryService = candleQueryService;
        }

        public async Task<List<MarketCandle>> CreateNearestDueDateContractIndex(
            string symbolStart,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellation)
        {
            var symbols = await GetOrderedSymbols(symbolStart, cancellation);
            return await BuildForwardIndex(symbols, startDate, endDate, cancellation);
        }

        public async Task<List<MarketCandle>> CreateSecondDueDateContractIndex(
            string symbolStart,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellation)
        {
            var symbols = await GetOrderedSymbols(symbolStart, cancellation);

            var activeCount = symbols.Count(x => x.DeliveryDate >= DateTime.Now.ToDateOnly());
            symbols = symbols
                .Take(symbols.Count - activeCount + 2)
                .OrderByDescending(x => x.DeliveryDate)
                .ToList();

            return await BuildBackwardIndex(symbols, startDate, endDate, cancellation);
        }

        public async Task<List<MarketCandle>> CreateLatestDueDateContractIndex(
            string symbolStart,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellation)
        {
            var symbols = await GetOrderedSymbols(symbolStart, cancellation);
            symbols = symbols.OrderByDescending(x => x.DeliveryDate).ToList();

            return await BuildBackwardIndex(symbols, startDate, endDate, cancellation);
        }

        public async Task<List<MarketCandle>> CreateIndexByContractRank(
            string symbolStart,
            DateTime startDate,
            DateTime endDate,
            int rank,
            CancellationToken cancellation)
        {
            var symbols = await GetOrderedSymbols(symbolStart, cancellation);

            if (rank < 1 || rank > symbols.Count)
                return new List<MarketCandle>();

            var selected = symbols.Take(rank).ToList();

            var allCandles = await GetCandles(selected.Select(x => x.Symbol), startDate, endDate, cancellation);

            var candlesBySymbol = allCandles
                .GroupBy(x => x.InstrumentId)
                .ToDictionary(x => x.Key, x => x.OrderBy(c => c.Date).ToList());

            var result = new List<MarketCandle>();

            for (int i = 0; i < selected.Count; i++)
            {
                var symbol = selected[i];

                DateOnly windowStart =
                    i == 0
                    ? startDate.ToDateOnly()
                    : selected[i - 1].DeliveryDate.AddDays(1);

                DateOnly windowEndExclusive = symbol.DeliveryDate;

                if (!candlesBySymbol.TryGetValue(symbol.Symbol, out var candles))
                    continue;

                var filtered = candles
                    .Where(c =>
                        c.Date.ToDateOnly() >= windowStart &&
                        c.Date.ToDateOnly() < windowEndExclusive)
                    .ToList();

                result.AddRange(filtered);
            }

            return result;
        }

        private async Task<List<ImeSymbol>> GetOrderedSymbols(
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

        private async Task<List<MarketCandle>> GetCandles(
            IEnumerable<string> symbols,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellation)
        {
            var dailyCandles = await _candleQueryService.GetCandlesAsync(
                new CandleQuery
                {
                    From = startDate,
                    To = endDate,
                    Timeframe = Timeframe.Daily,
                    SymbolKeys = symbols.ToArray(),
                    Markets = new[] { MarketType.Ime }
                },
                cancellation);

            return dailyCandles.Select(e => new MarketCandle(e)).ToList();

            //return await _imeService.GetCandlesAsync(
            //    new MarketQueryBuilder()
            //        .Between(startDate, endDate)
            //        .ForSymbols(symbols.ToArray())
            //        .Build(),
            //    cancellation);
        }

        private async Task<List<MarketCandle>> BuildForwardIndex(
            List<ImeSymbol> symbols,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellation)
        {
            var allCandles = await GetCandles(
                symbols.Select(x => x.Symbol),
                startDate,
                endDate,
                cancellation);

            var candlesBySymbol = allCandles
                .GroupBy(x => x.InstrumentId)
                .ToDictionary(x => x.Key, x => x.OrderBy(c => c.Date).ToList());

            var result = new List<MarketCandle>();

            DateTime? lastDate = null;

            foreach (var symbol in symbols)
            {
                if (!candlesBySymbol.TryGetValue(symbol.Symbol, out var candles))
                    continue;

                if (lastDate.HasValue)
                {
                    candles = candles
                        .Where(c => c.Date.Date >= lastDate.Value.Date.AddDays(1))
                        .ToList();
                }

                result.AddRange(candles);

                if (candles.Count > 0)
                    lastDate = candles.Max(x => x.Date);
            }

            return result;
        }

        private async Task<List<MarketCandle>> BuildBackwardIndex(
            List<ImeSymbol> symbols,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellation)
        {
            var allCandles = await GetCandles(
                symbols.Select(x => x.Symbol),
                startDate,
                endDate,
                cancellation);

            var candlesBySymbol = allCandles
                .GroupBy(x => x.InstrumentId)
                .ToDictionary(x => x.Key, x => x.OrderByDescending(c => c.Date).ToList());

            var result = new List<MarketCandle>();

            DateTime? boundaryExclusive = endDate;

            foreach (var symbol in symbols)
            {
                if (!candlesBySymbol.TryGetValue(symbol.Symbol, out var candles))
                    continue;

                if (boundaryExclusive.HasValue)
                {
                    candles = candles
                        .Where(c => c.Date.Date < boundaryExclusive.Value.Date)
                        .ToList();
                }

                result.AddRange(candles);

                if (candles.Count > 0)
                    boundaryExclusive = candles.Min(x => x.Date).Date;
            }

            return result;
        }
    }
}


//using MarketAnalyzer.Domain;
//using MarketAnalyzer.Analysis.Infrastructure;
//using MarketAnalyzer.SharedKernel;

//namespace MarketAnalyzer.AnalysisStrategy
//{
//    public class CalculateImeContractIndex : ICalculateImeContractIndex
//    {
//        private readonly IImeService _imeService;
//        private List<ImeSymbol> _symbols = new();

//        public CalculateImeContractIndex(IImeService imeService)
//        {
//            _imeService = imeService;
//        }

//        private async Task<List<ImeSymbol>> GetSymbols(string symbolStart, CancellationToken cancellation)
//        {
//            _symbols = await _imeService.GetSymbolsAsync(e => e.Symbol.StartsWith(symbolStart), cancellation);

//            //var ss = _symbols.Where(e=>e.DeliveryDate>=DateTime.Now.ToDateOnly()).order
//            return _symbols;
//        }

//        /// <summary>
//        /// ایجاد ایندکس قرارداد، با نزدیکترین سر رسید
//        /// </summary>
//        public async Task<List<MarketCandle>> CreateNearestDueDateContractIndex(
//            string symbolStart,
//            DateTime startDate,
//            DateTime endDate,
//            CancellationToken cancellation)
//        {
//            var symbols = await _imeService.GetSymbolsAsync(e => e.Symbol.StartsWith(symbolStart), cancellation);

//            symbols = symbols
//                .OrderBy(e => e.DeliveryDate)
//                .ToList();

//            var symbolList = symbols.Select(e => e.Symbol);

//            var allCandles = await _imeService.GetCandlesAsync(new MarketQueryBuilder()
//               .Between(startDate, endDate)               
//               .WithTimeframe(TimeframeDto.Daily)
//               .ForSymbols(symbolList.ToArray())
//               .Build(), cancellation);

//            var result = new List<MarketCandle>();
//            DateTime? lastDate = null;

//            foreach (var s in symbols)
//            {
//                var listForSymbol = allCandles
//                    .Where(c => c.InstrumentId == s.Symbol)
//                    .OrderBy(c => c.Date)
//                    .ToList();

//                if (!listForSymbol.Any()) continue;

//                if (lastDate.HasValue)
//                {
//                    DateTime start = lastDate.Value.Date.AddDays(1);
//                    listForSymbol = listForSymbol.Where(c => c.Date.Date >= start).ToList();
//                }

//                foreach (var c in listForSymbol)
//                {
//                    result.Add(c);
//                }

//                if (listForSymbol.Any())
//                    lastDate = listForSymbol.Max(x => x.Date);
//            }

//            return result;
//        }

//        public async Task<List<MarketCandle>> CreateNearestDueDateContractIndex1(
//            string symbolStart,
//            DateTime startDate,
//            DateTime endDate, CancellationToken cancellation)
//        {
//            var symbols = await _imeService.GetSymbolsAsync(e => e.Symbol.StartsWith(symbolStart), cancellation);

//            var activeSymbols = symbols.Where(e => e.DeliveryDate >= DateTime.Now.ToDateOnly()).Count();

//            symbols = symbols
//                .OrderBy(e => e.DeliveryDate)
//                .Take(symbols.Count - activeSymbols + 1)
//                .OrderByDescending(e => e.DeliveryDate)
//                .ToList();

//            var symbolList = symbols.Select(e => e.Symbol);

//            var allCandles = await _imeService.GetCandlesAsync(new MarketQueryBuilder()
//               .Between(startDate, endDate)
//               .ForSymbols(symbolList.ToArray())
//               .Build(), cancellation);

//            var result = new List<MarketCandle>();
//            DateTime? boundaryExclusive = endDate; // محدودیت بالایی برای تاریخ‌های آینده

//            foreach (var s in symbols)
//            {
//                var listForSymbol = allCandles
//                    .Where(c => c.InstrumentId == s.Symbol)
//                    .OrderByDescending(c => c.Date)
//                    .ToList();

//                if (!listForSymbol.Any()) continue;

//                // فقط تاریخ‌هایی که قبل از مرز فعلی هستند را بگیر
//                if (boundaryExclusive.HasValue)
//                {
//                    listForSymbol = listForSymbol
//                        .Where(c => c.Date.Date < boundaryExclusive.Value.Date)
//                        .ToList();
//                }

//                foreach (var c in listForSymbol)
//                {
//                    result.Add(c);
//                }

//                if (listForSymbol.Any())
//                {
//                    // مرز را به کمترین تاریخ از این Symbol بروزرسانی کن تا به سمت قدیمی‌تر حرکت کنیم
//                    boundaryExclusive = listForSymbol.Min(x => x.Date).Date;
//                }
//            }

//            return result;
//        }

//        /// <summary>
//        /// ایجاد ایندکس قرارداد، سر رسید دوم
//        /// </summary>
//        public async Task<List<MarketCandle>> CreateSecondDueDateContractIndex(
//            string symbolStart,
//            DateTime startDate,
//            DateTime endDate, CancellationToken cancellation)
//        {
//            var symbols = await _imeService.GetSymbolsAsync(e => e.Symbol.StartsWith(symbolStart), cancellation);

//            var activeSymbols = symbols.Where(e => e.DeliveryDate >= DateTime.Now.ToDateOnly()).Count();

//            symbols = symbols
//                .OrderBy(e => e.DeliveryDate)
//                .Take(symbols.Count - activeSymbols + 2)
//                .OrderByDescending(e => e.DeliveryDate)
//                .ToList();

//            var symbolList = symbols.Select(e => e.Symbol);

//            var allCandles = await _imeService.GetCandlesAsync(new MarketQueryBuilder()
//               .Between(startDate, endDate)               
//               .ForSymbols(symbolList.ToArray())
//               .Build(), cancellation);

//            var result = new List<MarketCandle>();
//            DateTime? boundaryExclusive = endDate; // محدودیت بالایی برای تاریخ‌های آینده

//            foreach (var s in symbols)
//            {
//                var listForSymbol = allCandles
//                    .Where(c => c.InstrumentId == s.Symbol)
//                    .OrderByDescending(c => c.Date)
//                    .ToList();

//                if (!listForSymbol.Any()) continue;

//                // فقط تاریخ‌هایی که قبل از مرز فعلی هستند را بگیر
//                if (boundaryExclusive.HasValue)
//                {
//                    listForSymbol = listForSymbol
//                        .Where(c => c.Date.Date < boundaryExclusive.Value.Date)
//                        .ToList();
//                }

//                foreach (var c in listForSymbol)
//                {
//                    result.Add(c);
//                }

//                if (listForSymbol.Any())
//                {
//                    // مرز را به کمترین تاریخ از این Symbol بروزرسانی کن تا به سمت قدیمی‌تر حرکت کنیم
//                    boundaryExclusive = listForSymbol.Min(x => x.Date).Date;
//                }
//            }

//            return result;
//        }

//        /// <summary>
//        /// ایجاد ایندکس قرارداد با دورترین سررسید
//        /// </summary>
//        public async Task<List<MarketCandle>> CreateLatestDueDateContractIndex(
//            string symbolStart,
//            DateTime startDate,
//            DateTime endDate,
//            CancellationToken cancellation)
//        {
//            var symbols = await _imeService.GetSymbolsAsync(e => e.Symbol.StartsWith(symbolStart), cancellation);

//            // ترتیب بر اساس دورتی که دورترین سررسید دارد
//            symbols = symbols
//                .OrderByDescending(e => e.DeliveryDate)
//                .ToList();

//            var symbolList = symbols.Select(e => e.Symbol);

//            var allCandles = await _imeService.GetCandlesAsync(new MarketQueryBuilder()
//               .Between(startDate, endDate)
//               .ForSymbols(symbolList.ToArray())
//               .Build(), cancellation);

//            var result = new List<MarketCandle>();
//            DateTime? boundaryExclusive = endDate; // محدودیت بالایی برای تاریخ‌های آینده

//            foreach (var s in symbols)
//            {
//                var listForSymbol = allCandles
//                    .Where(c => c.InstrumentId == s.Symbol)
//                    .OrderByDescending(c => c.Date)
//                    .ToList();

//                if (!listForSymbol.Any()) continue;

//                // فقط تاریخ‌هایی که قبل از مرز فعلی هستند را بگیر
//                if (boundaryExclusive.HasValue)
//                {
//                    listForSymbol = listForSymbol
//                        .Where(c => c.Date.Date < boundaryExclusive.Value.Date)
//                        .ToList();
//                }

//                foreach (var c in listForSymbol)
//                {
//                    result.Add(c);
//                }

//                if (listForSymbol.Any())
//                {
//                    // مرز را به کمترین تاریخ از این Symbol بروزرسانی کن تا به سمت قدیمی‌تر حرکت کنیم
//                    boundaryExclusive = listForSymbol.Min(x => x.Date).Date;
//                }
//            }

//            return result;
//        }

//        public async Task<List<MarketCandle>> CreateIndexByContractRank(
//            string symbolStart,
//            DateTime startDate,
//            DateTime endDate,
//            int rank,            // 1-based: 1 = اولین/Nearest, 2 = دومین, ...
//            CancellationToken cancellation)
//        {
//            // 1) نمادها رو مطابق DeliveryDate مرتب می‌گیریم
//            var symbols = await _imeService.GetSymbolsAsync(e => e.Symbol.StartsWith(symbolStart), cancellation);
//            symbols = symbols.OrderBy(e => e.DeliveryDate).ToList();

//            // 2) rank معتبر است؟
//            if (rank < 1 || rank > symbols.Count)
//                return new List<MarketCandle>();

//            // 3) انتخاب نمادهای تا رنک مشخص
//            var selected = symbols.Take(rank).ToList();
//            var symbolList = selected.Select(s => s.Symbol).ToList();

//            // 4) کندل‌های مربوط به همه این نمادها در بازه کل را می‌گیریم
//            var allCandles = await _imeService.GetCandlesAsync(new MarketQueryBuilder()
//               .Between(startDate, endDate)
//               .ForSymbols(symbolList.ToArray())
//               .Build(), cancellation);

//            // 5) برای هر قرارداد، بازه خودش را می‌سازیم: 
//            //     ابزار i: windowStart = (i == 0) ? startDate : previous.DeliveryDate.AddDays(1)
//            //                windowEndExclusive = current.DeliveryDate
//            var result = new List<MarketCandle>();

//            for (int i = 0; i < rank; i++)
//            {
//                var s = selected[i].Symbol;
//                DateOnly windowStart = (i == 0) ? startDate.ToDateOnly() : selected[i - 1].DeliveryDate.AddDays(1);
//                DateOnly windowEndExclusive = selected[i].DeliveryDate;

//                // اگر تاریخ آغاز از endDate عبور کرد، بریز بیرون
//                if (windowStart > endDate.ToDateOnly()) break;

//                // محدود کردن به بازه پایان=endDate
//                if (windowEndExclusive > endDate.AddDays(1).ToDateOnly()) windowEndExclusive = endDate.AddDays(1).ToDateOnly();

//                var listForSymbol = allCandles
//                    .Where(c => c.InstrumentId == s && c.Date.ToDateOnly() >= windowStart && c.Date.ToDateOnly() < windowEndExclusive)
//                    .OrderBy(c => c.Date)
//                    .ToList();

//                result.AddRange(listForSymbol);
//            }

//            return result;
//        }
//    }
//}
