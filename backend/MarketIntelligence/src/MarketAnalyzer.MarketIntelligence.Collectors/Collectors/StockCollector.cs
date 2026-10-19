using MarketAnalyzer.MarketIntelligence.Domain;
using MarketAnalyzer.MarketIntelligence.Infrastructure;

namespace MarketAnalyzer.MarketIntelligence.Collectors
{
    public class StockCollector : BaseCollector
    {
        public override string CollectorName => GetType().Name;
        private readonly ITseTmcClient _tseClient;
        private readonly IStockService _stockService;

        public StockCollector(ITseTmcClient tseClient, IStockService stockService)
        {
            _tseClient = tseClient;
            _stockService = stockService;
        }


        ///// <summary>
        ///// گرفتن و ذخیره دیتای ایندکس بازار تهران
        ///// </summary>
        //private async Task FetchEveryWeek()
        //{
        //    // حذف کل کندل های ایندکس بورس تهران
        //    await _tseTmcIndexService.DeleteDuplicateDataAsync(cancellation: CancellationToken.None);

        //    var data = await _tseTmcIndexService.GetSymbolsAsync(null, cancellation: CancellationToken.None);
        //    var candlesData = new List<CandleSeries>();

        //    foreach (var index in data)
        //    {
        //        var candle = await _tseClient.GetIndexCandleByInsCode(index.InsCode);

        //        candlesData.Add(candle);
        //    }
        //    await _indexCandleBulkService.SaveOrUpdateBulkCandlesAsync(candlesData, CancellationToken.None);
        //}


        public async Task FetchTradeLastDay()
        {
            var data = await _tseClient.GetTradeLastDayAsync(cancellation: CancellationToken.None);

            await _stockService.SaveTseTradeLastDayAsync(data, cancellation: CancellationToken.None);
        }

        public async Task FetchTseStaticThreshold()
        {
            var data = await _tseClient.GetStaticThresholdAsync(cancellation: CancellationToken.None);

            await _stockService.SaveTseStaticThresholdAsync(data, cancellation: CancellationToken.None);
        }

        public async Task FetchTseBestLimit()
        {
            var data = await _tseClient.GetBestLimitsAsync(cancellation: CancellationToken.None);

            await _stockService.SaveTseBestLimitAsync(data, cancellation: CancellationToken.None);
        }

        public async Task FetchETFByInsCode()
        {
            var ss = await _tseClient.GetETFByInsCode("58514988269776425");

            //await _tseTmcService.save(ss, cancellation: CancellationToken.None);
        }
        public async Task FetchMofidEasyFilterData()
        {
            var ss = await _tseClient.GetMofidEasyFilterData();

            //await _tseTmcService.save(ss, cancellation: CancellationToken.None);
        }

        //public async Task FetchNewData()
        //{
        //    var ss = await _tseClient.GetIndexesAsync(cancellation: CancellationToken.None);

        //    //await _tseTmcService.save(ss, cancellation: CancellationToken.None);
        //}
    }
}

