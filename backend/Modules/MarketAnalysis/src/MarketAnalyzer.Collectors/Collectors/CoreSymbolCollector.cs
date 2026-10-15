using MarketAnalyzer.Collector;
using MarketAnalyzer.Domain;
using MarketAnalyzer.Analysis.Infrastructure;
using MarketAnalyzer.SharedKernel;
using System.Text.Json;

namespace MarketAnalyzer.Collectors
{
    public class CoreSymbolCollector : BaseCollector
    {
        public override string CollectorName => GetType().Name;

        private readonly ITseTmcClient _tseClient;
        private readonly IBaseImeFutureSymbolsProvider _baseImeFutureSymbolsProvider;
        private readonly ICryptoSpotSymbolsProvider _cryptoSpotSymbolsProvider;
        private readonly IInvestmentFundClient _investmentFundClient;
        private readonly IInvestmentFundService _investmentFundService;
        private readonly IUnifiedSymbolService _unifiedSymbolService;
        private readonly IBaseImeFutureSymbolsProvider _imeFuture;
        private readonly IUnifiedSymbolRepository _unifiedSymbolRepository;

        public CoreSymbolCollector(
            ITseTmcClient tseClient,
            IBaseImeFutureSymbolsProvider baseImeFutureSymbolsProvider,
            ICryptoSpotSymbolsProvider cryptoSpotSymbolsProvider,
            IInvestmentFundClient investmentFundClient,
            IInvestmentFundService investmentFundService,
            IUnifiedSymbolService unifiedSymbolService,
            IBaseImeFutureSymbolsProvider imeFuture,
            IUnifiedSymbolRepository unifiedSymbolRepository)
        {
            _tseClient = tseClient;
            _baseImeFutureSymbolsProvider = baseImeFutureSymbolsProvider;
            _cryptoSpotSymbolsProvider = cryptoSpotSymbolsProvider;
            _investmentFundClient = investmentFundClient;
            _investmentFundService = investmentFundService;
            _unifiedSymbolService = unifiedSymbolService;
            _imeFuture = imeFuture;
            _unifiedSymbolRepository = unifiedSymbolRepository;
        }

        public async Task FetchSymbols()
        {
            await FetchTseSymbols();
            await FetchCryptoSymbols();
            await FetchFundSymbols();
            await FetchImeSymbols();
        }

        /// <summary>
        /// بورس ایران
        /// </summary>
        public async Task FetchTseSymbols()
        {
            var data = await _tseClient.GetStockInstrumentAsync(cancellation: CancellationToken.None);

            await _unifiedSymbolService.SaveMappedSymbolsAsync(
                data,
                x => x != null,
                x => x.ToUnifiedSymbol(),
                 CancellationToken.None);
        }

        /// <summary>
        /// کریپتو
        /// </summary>
        public async Task FetchCryptoSymbols()
        {
            var data = await _cryptoSpotSymbolsProvider.GetSpotSymbolsAsync();

            await _unifiedSymbolService.SaveMappedSymbolsAsync(
                data,
                x => x != null,
                x => x.ToUnifiedSymbol(),
                 CancellationToken.None);
        }

        /// <summary>
        /// صندوق های سرمایه
        /// </summary>
        public async Task FetchFundSymbols()
        {
            var data = await _investmentFundClient.GetInvestmentFund(cancellation: CancellationToken.None);

            await _unifiedSymbolService.SaveMappedSymbolsAsync(
                data,
                x => x != null,
                x => x.ToUnifiedSymbol(),
                 CancellationToken.None);
        }

        /// <summary>
        /// کالا
        /// </summary>
        public async Task FetchImeSymbols()
        {
            var symbols = await _unifiedSymbolRepository
                .GetAllAsync(MarketType.Ime, CancellationToken.None);

            var symbolKeys = symbols
                .Select(e => new
                {
                    e.SymbolKey,
                    e.PersianName,
                    e.InsCode,
                    Metadata = e.MetadataJson
                })
                .AsEnumerable() // از اینجا به بعد LINQ در حافظه اجرا می‌شود
                .Where(e =>
                {
                    if (string.IsNullOrWhiteSpace(e.Metadata))
                        return false;

                    var meta = JsonSerializer.Deserialize<ImeSymbolMetadata>(e.Metadata, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return meta != null
                        && meta.DeliveryDate > DateTime.Now.AddYears(-20).ToDateOnly();
                })
                .Distinct()
                .ToList();

            var minNumber = symbolKeys.Min(e => e.InsCode);
            var countNumber = symbolKeys.Count();
            var numbers = Enumerable
                .Range(0, countNumber * 2)
                .Select(i => Convert.ToString(Convert.ToInt32(minNumber) + (long)i))
                .ToList();

            var listIme = new List<ImeSymbol>();
            foreach (var number in numbers)
            {
                var data = await _imeFuture.GetSymbolsAsync(
                    number,Timeframe.Daily,5,
               cancellation: CancellationToken.None);

                if (data is not null)
                {
                    listIme.Add(data);
                }                
            }

            await _unifiedSymbolService.SaveMappedSymbolsAsync(
                listIme,
                x => x != null,
                x => x.ToUnifiedSymbol(),
                 CancellationToken.None);
        }




        // TODO 


        /// <summary>
        /// خانه
        /// </summary>
        public async Task FetchRealEstateSymbols()
        {
            //var data = await _investmentFundClient.GetInvestmentFund(cancellation: CancellationToken.None);

            //await _unifiedSymbolService.SaveMappedSymbolsAsync(
            //    data,
            //    x => x != null,
            //    x => x.ToUnifiedSymbol(),
            //     CancellationToken.None);
        }

        // اسم صندوق ها
        private async Task FetchInvestmentFund()
        {
            var investmentFund = await _investmentFundClient.GetInvestmentFund(cancellation: CancellationToken.None);
            await _investmentFundService.SaveInvestmentFundAsync(investmentFund, cancellation: CancellationToken.None);
        }

        /// <summary>
        /// گرفتن نماد های پایه  - مثلا برای Ime
        /// </summary>
        /// <returns></returns>
        private async Task FetchBaseStockCandle(Timeframe timeframe)
        {
            var symbols = await _unifiedSymbolRepository
               .GetAllAsync(MarketType.Ime, CancellationToken.None);

            var symbolKeys = symbols
                .Select(e => new
                {
                    e.SymbolKey,
                    e.PersianName,
                    e.InsCode,
                    Metadata = e.MetadataJson
                })
                .AsEnumerable() // از اینجا به بعد LINQ در حافظه اجرا می‌شود
                .Where(e =>
                {
                    if (string.IsNullOrWhiteSpace(e.Metadata))
                        return false;

                    var meta = JsonSerializer.Deserialize<ImeSymbolMetadata>(e.Metadata, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return meta != null
                        && meta.DeliveryDate > DateTime.Now.AddYears(-20).ToDateOnly();
                })
                .Distinct()
                .ToList();

            //const int chunkSize = 2;

            //_yearsAgo.TryGetValue(timeframe, out var yearsAgo);

            //// تقسیم نمادها به چانک‌های 50 تایی
            //var symbolChunks = symbols
            //    .Where(e => e.ParentSymbol is not null)
            //    .Select(e => e.ParentSymbol)
            //    .Distinct()
            //    .Chunk(chunkSize);

            //await ProcessSymbolsAsync(
            //    symbolChunks,
            //    _timeFrame,
            //    MarketTypeDto.TehranExchenge_Nahayatnegar,
            //    yearsAgo,
            //    _marketCollector,
            //    _stockCandleBulkService,
            //    CancellationToken.None
            //);

            var candlesData = new List<CandleSeries>();
            var data = await _baseImeFutureSymbolsProvider.GetBaseImeFutureCandlesAsync("silver");

            candlesData.Add(new CandleSeries
            {
                Candels = data,
                marketType = (int)MarketType.Ime,
                Symbol = "BaseIme-Silver",
                Timeframe = Timeframe.Daily
            });

            //await _imeCandleBulkService.SaveOrUpdateBulkCandlesAsync(candlesData, CancellationToken.None);
        }
    }
}
