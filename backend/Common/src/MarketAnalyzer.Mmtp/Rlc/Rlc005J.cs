namespace MarketAnalyzer.Mmtp
{
    /// <summary>
    /// Closing Price
    /// </summary>
    public class Rlc005J : RlcMessageBase
    {
        public Rlc005J(byte[] buffer) : base(buffer)
        {
            ClosingPrice = MmtpConvert.GetDecimal(buffer, HeaderLength + 1, 14);
            IsClosePrice = MmtpConvert.GetBoolean(buffer, HeaderLength + 15);
            ClosingPriceType = MmtpConvert.GetString(buffer, HeaderLength + 16, 1);
            NotAdjustedClosingPrice = MmtpConvert.GetDecimal(buffer, HeaderLength + 17, 14);
            ValueWeightedAveragePrice = MmtpConvert.GetDecimal(buffer, HeaderLength + 31, 14);
            NotAdjustedValueWeightedAveragePrice = MmtpConvert.GetDecimal(buffer, HeaderLength + 45, 14);
            LastTradePrice = MmtpConvert.GetDecimal(buffer, HeaderLength + 59, 14);
            NotAdjustedLastTradePrice = MmtpConvert.GetDecimal(buffer, HeaderLength + 73, 14);
            TotalTradesNumber = MmtpConvert.GetInt64(buffer, HeaderLength + 87, 12);
            TotalTradedQuantity = MmtpConvert.GetInt64(buffer, HeaderLength + 99, 12);
            TotalTradeValue = MmtpConvert.GetDecimal(buffer, HeaderLength + 111, 16);
        }

        public decimal ClosingPrice { get; }
        public bool IsClosePrice { get; }
        public string ClosingPriceType { get; }
        public decimal NotAdjustedClosingPrice { get; }
        public decimal ValueWeightedAveragePrice { get; }
        public decimal NotAdjustedValueWeightedAveragePrice { get; }
        public decimal LastTradePrice { get; }
        public decimal NotAdjustedLastTradePrice { get; }
        public long TotalTradesNumber { get; }
        public long TotalTradedQuantity { get; }
        public decimal TotalTradeValue { get; }
    }
}