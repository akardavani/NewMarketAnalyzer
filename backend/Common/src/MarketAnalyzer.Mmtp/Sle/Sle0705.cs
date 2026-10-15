namespace MarketAnalyzer.Mmtp
{
    /// <summary>
    /// Results of Cash Settlement
    /// </summary>
    public class Sle0705 : SleMessageBase
    {
        public Sle0705(byte[] buffer) : base(buffer)
        {
            BrokersId = MmtpConvert.GetString(buffer, 20, 8);
            InstrumentId = MmtpConvert.GetString(buffer, 28, 12);
            ClientId = MmtpConvert.GetString(buffer, 40, 16);
            BuyOpenPosition = MmtpConvert.GetInt64(buffer, 56, 12);
            SellOpenPosition = MmtpConvert.GetInt64(buffer, 68, 12);
            RequestedNumberOfCashSettles = MmtpConvert.GetInt64(buffer, 80, 12);
            RequestAllowedToSettleWithAllOfAsset = MmtpConvert.GetBoolean(buffer, 92);
            RequestAllowedToSettleWithFractionAsset = MmtpConvert.GetBoolean(buffer, 93);
            CashSettlementsExecutedCount = MmtpConvert.GetInt64(buffer, 94, 12);
            CashSettlementsExecutedPositionCount = MmtpConvert.GetInt64(buffer, 106, 18);
        }

        public string BrokersId { get; }
        /// <summary>
        /// نماد دارایی قرارداد اختیار معامله
        /// </summary>
        public string InstrumentId { get; }
        /// <summary>
        /// کد مشتری
        /// </summary>
        public string ClientId { get; }
        /// <summary>
        /// تعداد موقعیت های باز خرید
        /// </summary>
        public long BuyOpenPosition { get; }
        /// <summary>
        /// تعداد موقعیت های باز فروش
        /// </summary>
        public long SellOpenPosition { get; }
        /// <summary>
        /// تعداد مورد درخواست برای تسویه نقدی
        /// </summary>
        public long RequestedNumberOfCashSettles { get; }
        /// <summary>
        /// درخواست تسویه فیزیکی تمام دارایی
        /// </summary>
        public bool RequestAllowedToSettleWithAllOfAsset { get; }
        /// <summary>
        /// درخواست تسویه نقدی با پاره سهم
        /// </summary>
        public bool RequestAllowedToSettleWithFractionAsset { get; }
        /// <summary>
        /// تعداد تسویه نقدی انجام شده
        /// </summary>
        public long CashSettlementsExecutedCount { get; }
        /// <summary>
        /// تعداد موقعیت هایی که تسویه نقدی شده اند
        /// </summary>
        public long CashSettlementsExecutedPositionCount { get; }
    }
}
