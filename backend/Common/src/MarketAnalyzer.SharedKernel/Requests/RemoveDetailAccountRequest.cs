namespace MarketAnalyzer.SharedKernel
{
    public class RemoveDetailAccountRequest
    {
        /// <summary>
        /// کد حساب تفصیلی
        /// </summary>
        public required string Code { get; init; }
        /// <summary>
        /// شناسه کاربر حذف کننده
        /// </summary>
        public required int OperatorId { get; init; }
    }
}