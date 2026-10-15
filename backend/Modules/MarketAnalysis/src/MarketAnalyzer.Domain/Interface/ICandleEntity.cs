namespace MarketAnalyzer.Domain
{
    public interface ICandleEntity
    {
        string Id { get; set; }
        string InstrumentId { get; set; }
        decimal Open { get; set; }
        decimal Close { get; set; }
        decimal High { get; set; }
        decimal Low { get; set; }
        decimal Volume { get; set; }
        int Timeframe { get; set; }
        DateTime Date { get; set; }
        string PersianDate { get; set; }
        long Timestamp { get; set; }
        decimal NetValue { get; set; }

        void LoadFrom(MarketCandle candle);
    }
}
