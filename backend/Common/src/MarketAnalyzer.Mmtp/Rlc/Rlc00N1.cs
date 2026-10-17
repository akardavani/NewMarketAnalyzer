using System;

namespace MarketAnalyzer.Mmtp
{
    public class Rlc00N1 : RlcMessageBase
    {
        public Rlc00N1(byte[] buffer) : base(buffer)
        {
            TicketNumber = MmtpConvert.GetInt32(buffer, HeaderLength + 9, 10);
            TradedQuantity = MmtpConvert.GetInt64(buffer, HeaderLength + 19, 12);
            Price = MmtpConvert.GetDecimal(buffer, HeaderLength + 31, 14);
            TradeDate = MmtpConvert.GetDateTime(buffer, HeaderLength + 63, 14);
            TradeCancellationFlag = MmtpConvert.GetTradeCancellationFlag(buffer, HeaderLength + 45);
            BuyerTraderId = MmtpConvert.GetString(buffer, HeaderLength + 202, 8).Trim();
            BuyerClientType = MmtpConvert.GetString(buffer, HeaderLength + 210, 1).Trim();
            BuyerAccount = MmtpConvert.GetString(buffer, HeaderLength + 221, 16).Trim();
            SellerTraderId = MmtpConvert.GetString(buffer, HeaderLength + 401, 8).Trim();
            SellerClientType = MmtpConvert.GetString(buffer, HeaderLength + 409, 1).Trim();
            SellerAccount = MmtpConvert.GetString(buffer, HeaderLength + 420, 16).Trim();
        }

        public string BuyerAccount { get; }
        public string BuyerClientType { get; }
        public string BuyerTraderId { get; }
        public string SellerAccount { get; }
        public string SellerTraderId { get; }
        public string SellerClientType { get; }
        public TradeCancellationFlag TradeCancellationFlag { get; }
        public DateTime TradeDate { get; }
        public decimal Price { get; }
        public long TradedQuantity { get; }
        public int TicketNumber { get; }
    }
}