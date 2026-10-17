using MarketIntelligence.Domain;

namespace MarketIntelligence.Infrastructure
{
    public class StockService :  IStockService
    {
        private readonly IRepository<TseStaticThreshold> _tseStaticThresholdRepository;
        private readonly IRepository<TseBestLimit> _tseBestLimitRepository;
        private readonly IRepository<TseTradeLastDay> _tseTradeLastDayRepository;

        public StockService(
            IRepository<TseStaticThreshold> tseStaticThresholdRepository,
            IRepository<TseBestLimit> tseBestLimitRepository,
            IRepository<TseTradeLastDay> tseTradeLastDayRepository) 
        {
            _tseStaticThresholdRepository = tseStaticThresholdRepository;
            _tseBestLimitRepository = tseBestLimitRepository;
            _tseTradeLastDayRepository = tseTradeLastDayRepository;
        }

        /// <summary>
        /// ذخیره آستانه قیمتی 
        /// </summary>
        public async Task SaveTseStaticThresholdAsync(List<TseStaticThreshold> data, CancellationToken cancellation)
        {
            await _tseStaticThresholdRepository.AddRangeAsync(data, cancellation);
        }

        /// <summary>
        /// ذخیره قیمت های سرخطی 
        /// </summary>
        public async Task SaveTseBestLimitAsync(List<TseBestLimit> data, CancellationToken cancellation)
        {
            await _tseBestLimitRepository.AddRangeAsync(data, cancellation);
        }

        /// <summary>
        /// ذخیره 
        /// </summary>
        public async Task SaveTseTradeLastDayAsync(List<TseTradeLastDay> data, CancellationToken cancellation)
        {
            await _tseTradeLastDayRepository.AddRangeAsync(data, cancellation);
        }
    }
}

