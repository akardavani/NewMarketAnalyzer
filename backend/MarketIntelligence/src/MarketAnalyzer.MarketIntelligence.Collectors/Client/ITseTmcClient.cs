using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Collectors
{
    public interface ITseTmcClient
    {
        Task<List<TseTradeLastDay>> GetTradeLastDayAsync(CancellationToken cancellation = default);
        Task<List<TseStaticThreshold>> GetStaticThresholdAsync(CancellationToken cancellation = default);
        Task<List<TseBestLimit>> GetBestLimitsAsync(CancellationToken cancellation = default);
        Task<List<TseSymbol>> GetStockInstrumentAsync(CancellationToken cancellation = default);
        Task<TseTmcETFNavInfo> GetETFByInsCode(string insCode);

        Task<TseTmcETFNavInfo> GetMofidEasyFilterData();
        //Task<List<TseTradeLastDay>> GetIndexesAsync(CancellationToken cancellation = default);

        Task<List<TseIndexSymbol>> GetAllIndexAsync(CancellationToken cancellation = default);
        Task<CandleSeries> GetIndexCandleByInsCode(string insCode);
        Task<List<TseClientTypeInfo>> GetClientTypeAsync(string insCode);
    }
}
