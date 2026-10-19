public class MarketWatchItem
{
    public long InsCode { get; set; }

    public string Symbol { get; set; }

    public decimal LastPrice { get; set; }

    public decimal ClosingPrice { get; set; }

    public decimal PriceChange { get; set; }

    public decimal PercentChange { get; set; }

    public long Volume { get; set; }

    public long TradeCount { get; set; }

    public long Value { get; set; }
}
