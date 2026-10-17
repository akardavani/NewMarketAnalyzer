namespace MarketIntelligence.Domain
{
    public class MarketCandle
    {
        public MarketCandle() { }

        // سازنده عمومی برای هر نوع Candle که از CandleEntityBase ارث‌بری کرده
        public MarketCandle(CandleEntityBase source)
        {
            Id = source.Id;
            InstrumentId = source.InstrumentId;
            Open = source.Open;
            Close = source.Close;
            High = source.High;
            Low = source.Low;
            Volume = source.Volume;
            Timeframe = source.Timeframe;
            Date = source.Date;
            PersianDate = source.PersianDate;
            Timestamp = source.Timestamp;
            NetValue = source.NetValue;
            MarketType = source.MarketType;
        }

        public MarketCandle(UnifiedCandleRecord source)
        {
            Id = source.Id;
            InstrumentId = source.SymbolKey;
            Open = source.Open;
            Close = source.Close;
            High = source.High;
            Low = source.Low;
            Volume = source.Volume??0;
            Timeframe = (int)source.TimeframeId;
            Date = source.Date;
            PersianDate = source.PersianDate;
            //Timestamp = source.Timestamp;
            NetValue = source.Value??0;
            MarketType = (int)source.MarketId;
        }

        public string Id { get; set; }
        public string InstrumentId { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Volume { get; set; }
        public DateTime Date { get; set; }
        public decimal NetValue { get; set; }

        // custom
        public int Timeframe { get; set; }
        public int MarketType { get; set; }
        public long Timestamp { get; set; }
        public DateTime UtcDateTime => TimeZoneInfo.ConvertTimeToUtc(Date, TimeZoneInfo.Local);
        public string PersianDate { get; set; }
        public decimal Range
        {
            get { return High - Low; }
            //set { _size = value; }
        }
        public decimal Body => Math.Abs(Open - Close);

        // سبز
        public bool IsBullish
        {
            get { return Close > Open ? true : false; }
            //set { _isBullish = value; }
        }
        // قرمز
        public bool IsBearish
        {
            get { return Close < Open ? true : false; }
        }
        // فیتیله بالایی
        public decimal UpperWick
        {
            get { return High - Math.Max(Close, Open); }
        }

        // فیتیله پایینی
        public decimal LowerWick
        {
            get { return Math.Min(Close, Open) - Low; }
        }

        public bool Rally
        {
            get { return IsBullish && Body > UpperWick + LowerWick ? true : false; }
        }

        public bool Drop
        {
            get { return IsBearish && Body > UpperWick + LowerWick ? true : false; }
        }

        public bool Base
        {
            get { return Body < UpperWick + LowerWick ? true : false; }
        }

        public double BodyPct => (double)Body / (double)Range;
        public double UpperWickPct => (double)UpperWick / (double)Range;
        public double LowerWickPct => (double)LowerWick / (double)Range;
    }
}
