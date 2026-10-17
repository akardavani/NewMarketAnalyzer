namespace MarketIntelligence.Domain
{
    public abstract class CandleEntityBase : IEntity, ICandleEntity
    {
        public string Id { get; set; }
        public string InstrumentId { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Volume { get; set; }
        public int Timeframe { get; set; }
        public DateTime Date { get; set; }
        public string PersianDate { get; set; }
        public long Timestamp { get; set; }
        public decimal NetValue { get; set; }
        public int MarketType { get; set; }

        public virtual void LoadFrom(MarketCandle candle)
        {
            Id = candle.Id;
            InstrumentId = candle.InstrumentId;
            Open = candle.Open;
            Close = candle.Close;
            High = candle.High;
            Low = candle.Low;
            Volume = candle.Volume;
            Timeframe = candle.Timeframe;
            Date = candle.Date;
            PersianDate = candle.PersianDate;
            Timestamp = candle.Timestamp;
            NetValue = candle.NetValue;
            MarketType = candle.MarketType;
        }
    }
}
