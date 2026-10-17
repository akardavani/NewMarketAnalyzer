
public interface ITsetmcApi
{
    Task<List<MarketWatchItem>> GetMarketAsync();

    //Task<ClientTypeInfo> GetClientTypeAsync(long insCode);

    Task<List<TradeHistoryItem>> GetHistoryAsync(long insCode);
}


