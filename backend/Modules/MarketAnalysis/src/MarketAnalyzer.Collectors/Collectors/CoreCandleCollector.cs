using MarketAnalyzer.Domain;
using MarketAnalyzer.Analysis.Infrastructure;
using System.Text.Json;

namespace MarketAnalyzer.Collectors
{
    public class CoreCandleCollector : BaseCollector
    {
        public override string CollectorName => GetType().Name;

        private readonly IUnifiedSymbolRepository _unifiedSymbolRepository;
        private readonly ICandleSourceFactory _candleSourceFactory;
        private readonly ICandleIngestionService _candleIngestionService;
        private readonly IInvestmentFundClient _investmentFundClient;
        private readonly IInvestmentFundService _investmentFundService;
        private readonly IRepositoryFactory _repoFactory;
        private readonly ITseTmcClient _tseTmcClient;

        public CoreCandleCollector(
            IUnifiedSymbolRepository unifiedSymbolRepository,
            ICandleSourceFactory candleSourceFactory,
            ICandleIngestionService candleIngestionService,
            IInvestmentFundClient investmentFundClient,
             IInvestmentFundService investmentFundService,
             IRepositoryFactory repoFactory,
             ITseTmcClient tseTmcClient)
        {
            _unifiedSymbolRepository = unifiedSymbolRepository;
            _candleSourceFactory = candleSourceFactory;
            _candleIngestionService = candleIngestionService;
            _investmentFundClient = investmentFundClient;
            _investmentFundService = investmentFundService;
            _repoFactory = repoFactory;
            _tseTmcClient = tseTmcClient;
        }

        /// <summary>
        /// گرفتن هفتگی کندل های بازار تهران
        /// </summary>
        /// <returns></returns>
        public async Task FetchWeeklyCandles()
        {
            // حذف کندل های بازار تهران - روزانه
            await _candleIngestionService
                .BulkDeleteAsync(MarketType.Tse, Timeframe.Daily, CancellationToken.None);

            // حذف کندل های بازار تهران - ماهانه
            await _candleIngestionService
                .BulkDeleteAsync(MarketType.Tse, Timeframe.Mountly, CancellationToken.None);

            // گرفتن کندل از 5 سال گذشته
            await FetchTseCandle(yearsAgo: 5);
        }


        //public async Task<StandardFinancialData> GetFinancialReportAsync(long insCode, string reportType, CancellationToken ct)
        //{
        //    // reportType مثلا "6/6/0" برای ترازنامه یا "6/6/1" برای سود و زیان
        //    string url = $"https://api.tsetmc.com/api/Codal/GetStatementContentByInsCode?insCode={insCode}&reportType={reportType}";

        //    var result = await _fetcher.GetAsync<CodalReportDto>(url); // استفاده از همان متد Generic که پیشنهاد دادم

        //    // اینجا Mapper را صدا می‌زنیم (همان کدی که قبلاً نوشتیم)
        //    return new FinancialMapper().Normalize(insCode, result);
        //}

        /// <summary>
        /// گرفتن روزانه کندل همه بازار ها
        /// </summary>
        /// <returns></returns>
        public async Task FetchCandles()
        {
            // گرفتن کندل از 1 سال گذشته
            var yearsAgo = 1;

            // بازار سهام تهران
            await FetchTseCandle(yearsAgo: yearsAgo);
            // شاخص صنایع تهران
            await FetchTseIndexCandle(yearsAgo: yearsAgo);
            // کریپتو
            await FetchCryptoCandle(yearsAgo: yearsAgo);
            // طلا و دلار
            await FetchMacroCandle(yearsAgo: yearsAgo);
            // کریپتو - تومان
            await FetchCryptoTomanCandle(yearsAgo: yearsAgo);
            // خانه
            await FetchRealEstateCandle(yearsAgo: yearsAgo);
            // آتی 
            await FetchImeCandle(yearsAgo: 5);

            // دیتای معاملات حقیقی و حقوقی
            await FetchClientType();
        }

        /// <summary>
        /// دیتای معاملات حقیقی و حقوقی
        /// </summary>
        public async Task FetchClientType()
        {
            // ۱. دریافت لیست تمام نمادهای بورس
            var symbols = await GetTseSymbols();

            var insCodes = symbols.Select(e => e.InsCode).Where(c => !string.IsNullOrEmpty(c)).ToList();

            // ۲. تقسیم کدها به دسته‌های ۵۰ تایی برای پردازش بهینه
            int chunkSize = 20;
            for (int i = 0; i < insCodes.Count; i += chunkSize)
            {
                var currentChunk = insCodes.Skip(i).Take(chunkSize).ToList();
                var allCapturedData = new List<TseClientTypeInfo>();

                foreach (var insCode in currentChunk)
                {
                    var apiData = await _tseTmcClient.GetClientTypeAsync(insCode);

                    if (apiData != null && apiData.Any())
                    {
                        allCapturedData.AddRange(apiData);
                    }
                }

                // ۳. ذخیره دسته‌ای رکوردهای این Chunk بدون تکرار
                if (allCapturedData.Any())
                {
                    var minDate = allCapturedData.Min(x => x.RecDate);
                    var maxDate = allCapturedData.Max(x => x.RecDate);
                    var chunkInsCodes = allCapturedData.Select(x => x.InsCode).Distinct().ToList();

                    var clientTypeRepository = _repoFactory.Get<TseClientTypeInfo>();
                    await clientTypeRepository.BulkSaveWithMatchAsync(
                        entities: allCapturedData,
                        keySelector: x => (x.InsCode, x.RecDate),
                        // فیلتر دیتابیس برای چک کردن تکراری‌ها در همین بازه و همین نمادها
                        dbFilter: x => chunkInsCodes.Contains(x.InsCode) && x.RecDate >= minDate && x.RecDate <= maxDate,
                        dbKeySelector: x => (x.InsCode, x.RecDate),
                        cancellationToken: CancellationToken.None
                    );
                }
            }
        }


        /// <summary>
        /// گرفتن نماد بازار کالا
        /// </summary>
        private async Task FetchImeCandle(int yearsAgo)
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
                //.Select(x => x.SymbolKey)
                .Distinct()
                .ToList();

            var minNumber = symbolKeys.Min(e => e.InsCode);
            var countNumber = symbolKeys.Count();
            var numbers = Enumerable
                .Range(0, countNumber * 2)
                .Select(i => Convert.ToString(Convert.ToInt32(minNumber) + (long)i))
                .ToList();

            await ProcessSymbolsAsync(
                numbers,
                timeframe: Timeframe.Daily,
                marketType: MarketType.Ime,
                sourceType: SourceType.Ime,
                yearsAgo: yearsAgo,
                factory: _candleSourceFactory,
                ingestion: _candleIngestionService,
                cancellation: CancellationToken.None);
        }

        /// <summary>
        /// بورس ایران
        /// </summary>
        public async Task FetchTseCandle(int yearsAgo)
        {
            var symbols = await GetTseSymbols();
            var filterSymbols = symbols
                .Select(e => e.SymbolKey)
                .ToList();

            // دیتای روزانه 
            await ProcessSymbolsAsync(
                filterSymbols,
                timeframe: Timeframe.Daily,
                marketType: MarketType.Tse,
                sourceType: SourceType.Nahayatnegar,
                yearsAgo: yearsAgo,
                factory: _candleSourceFactory,
                ingestion: _candleIngestionService,
                cancellation: CancellationToken.None);

            // دیتای ماهانه
            await ProcessSymbolsAsync(
                filterSymbols,
                timeframe: Timeframe.Mountly,
                marketType: MarketType.Tse,
                sourceType: SourceType.Nahayatnegar,
                yearsAgo: yearsAgo,
                factory: _candleSourceFactory,
                ingestion: _candleIngestionService,
                cancellation: CancellationToken.None);
        }

        private async Task<List<UnifiedSymbolRecord>> GetTseSymbols()
        {
            var symbols = await _unifiedSymbolRepository
                .GetAllAsync(MarketType.Tse, CancellationToken.None);

            var symbolKeys = symbols
                .Where(e => e.SymbolKey.EndsWith("0001") &&
                            e.SymbolKey.StartsWith("IRO") &&
                            e.IsActive)
                //.Select(e => new
                //{
                //    e.SymbolKey,
                //    e.PersianName,
                //    Metadata = e.MetadataJson
                //})
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
                //.Select(x => x.SymbolKey)
                .Distinct()
                .ToList();

            return symbolKeys;
        }

        /// <summary>
        /// کریپتو
        /// </summary>
        public async Task FetchCryptoCandle(int yearsAgo)
        {
            var symbols = await _unifiedSymbolRepository
                .GetAllAsync(MarketType.Crypto, CancellationToken.None);

            var symbolKeys = symbols.Select(x => x.SymbolKey);

            await ProcessSymbolsAsync(
                symbolKeys,
                timeframe: Timeframe.Daily,
                marketType: MarketType.Crypto,
                sourceType: SourceType.Kucoin,
                yearsAgo: yearsAgo,
                factory: _candleSourceFactory,
                ingestion: _candleIngestionService,
                cancellation: CancellationToken.None);
        }

        /// <summary>
        /// Macro
        /// </summary>
        public async Task FetchMacroCandle(int yearsAgo)
        {
            var symbols = await _unifiedSymbolRepository
                .GetAllAsync(MarketType.Macro, CancellationToken.None);

            var symbolKeys = symbols.Select(x => x.SymbolKey);

            await ProcessSymbolsAsync(
                symbolKeys,
                timeframe: Timeframe.Daily,
                marketType: MarketType.Macro,
                sourceType: SourceType.tgju,
                yearsAgo: yearsAgo,
                factory: _candleSourceFactory,
                ingestion: _candleIngestionService,
                cancellation: CancellationToken.None);
        }

        /// <summary>
        /// CryptoToman
        /// </summary>
        public async Task FetchCryptoTomanCandle(int yearsAgo)
        {
            var symbols = await _unifiedSymbolRepository
                .GetAllAsync(MarketType.CryptoToman, CancellationToken.None);

            var symbolKeys = symbols.Select(x => x.SymbolKey);

            await ProcessSymbolsAsync(
                symbolKeys,
                timeframe: Timeframe.Daily,
                marketType: MarketType.CryptoToman,
                sourceType: SourceType.Wallex,
                yearsAgo: yearsAgo,
                factory: _candleSourceFactory,
                ingestion: _candleIngestionService,
                cancellation: CancellationToken.None);
        }

        /// <summary>
        /// RealEstate
        /// </summary>
        public async Task FetchRealEstateCandle(int yearsAgo)
        {
            var symbols = await _unifiedSymbolRepository
                .GetAllAsync(MarketType.RealEstate, CancellationToken.None);

            var symbolKeys = symbols.Select(x => x.SymbolKey);

            await ProcessSymbolsAsync(
                symbolKeys,
                timeframe: Timeframe.Mountly,
                marketType: MarketType.RealEstate,
                sourceType: SourceType.Kilid,
                yearsAgo: yearsAgo,
                factory: _candleSourceFactory,
                ingestion: _candleIngestionService,
                cancellation: CancellationToken.None);
        }

        /// <summary>
        /// شاخص
        /// </summary>
        public async Task FetchTseIndexCandle(int yearsAgo)
        {
            var symbols = await _unifiedSymbolRepository
                .GetAllAsync(MarketType.TseIndex, CancellationToken.None);

            var symbolKeys = symbols.Select(x => x.SymbolKey);

            await ProcessSymbolsAsync(
                symbolKeys,
                timeframe: Timeframe.Daily,
                marketType: MarketType.TseIndex,
                sourceType: SourceType.Nahayatnegar,
                yearsAgo: yearsAgo,
                factory: _candleSourceFactory,
                ingestion: _candleIngestionService,
                cancellation: CancellationToken.None);
        }

        ///// <summary>
        ///// صندوق های سرمایه
        ///// </summary>
        //public async Task FetchFundSymbols()
        //{
        //    var data = await _investmentFundClient.GetInvestmentFund(cancellation: CancellationToken.None);

        //    await _unifiedSymbolService.SaveMappedSymbolsAsync(
        //        data,
        //        x => x != null,
        //        x => x.ToUnifiedSymbol(),
        //         CancellationToken.None);
        //}


        /// <summary>
        /// گرفتن دیتای صندوق های بازار بورس
        /// </summary>
        private async Task FetchInvestmentFundChart(Timeframe timeframe)
        {
            await CalculateFundNavGap(cancellation: CancellationToken.None);

            var list = new List<InvestmentFundChart>();

            var investmentFund = await _investmentFundService.GetInvestmentFundAsync(null, cancellation: CancellationToken.None);
            foreach (var item in investmentFund)
            {
                var investmentFundChart = await _investmentFundClient.GetInvestmentFundChart(item.RegNo, cancellation: CancellationToken.None);
                list.AddRange(investmentFundChart);
            }

            await _investmentFundService.SaveInvestmentFundChartAsync(list, cancellation: CancellationToken.None);
        }

        private async Task CalculateFundNavGap(CancellationToken cancellation)
        {
            await _investmentFundService.CalculateFundNavGap(cancellation);
        }
    }
}
