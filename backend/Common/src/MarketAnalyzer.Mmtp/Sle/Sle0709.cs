namespace MarketAnalyzer.Mmtp
{
    /// <summary>
    /// Results of Physical Settlement
    /// </summary>
    public class Sle0709 : SleMessageBase
    {
        public Sle0709(byte[] buffer) : base(buffer)
        {
            BrokersId = MmtpConvert.GetString(buffer, 20, 8);
            InstrumentId = MmtpConvert.GetString(buffer, 28, 12);
            ClientId = MmtpConvert.GetString(buffer, 40, 16);
            BuyOpenPosition = MmtpConvert.GetInt64(buffer, 56, 12);
            SellOpenPosition = MmtpConvert.GetInt64(buffer, 68, 12);
            RequestedNumberOfPhysicalSettles = MmtpConvert.GetInt64(buffer, 80, 12);
            RequestAllowedToSettleWithAllOfAsset = MmtpConvert.GetBoolean(buffer, 92);
            RequestAllowedToSettleWithLoss = MmtpConvert.GetBoolean(buffer, 93);
            PhysicalSettlementsExecutedCount = MmtpConvert.GetInt64(buffer, 94, 12);
            PhysicalSettlementsDefaultedToCashSettlementCount = MmtpConvert.GetInt64(buffer, 106, 12);
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
        /// تعداد مورد درخواست برای تسویه فیزیکی
        /// </summary>
        public long RequestedNumberOfPhysicalSettles { get; }
        /// <summary>
        /// درخواست تسویه فیزیکی تمام دارایی
        /// </summary>
        public bool RequestAllowedToSettleWithAllOfAsset { get; }
        /// <summary>
        /// درخواست تسویه فیزیکی قرارداد در زیان یا بی تفاوت
        /// </summary>
        public bool RequestAllowedToSettleWithLoss { get; }
        /// <summary>
        /// تعداد تسویه فیزیکی انجام شده
        /// </summary>
        public long PhysicalSettlementsExecutedCount { get; }
        /// <summary>
        /// تعداد نکول شده منجر به تسویه نقدی
        /// </summary>
        public long PhysicalSettlementsDefaultedToCashSettlementCount { get; }
    }
}
