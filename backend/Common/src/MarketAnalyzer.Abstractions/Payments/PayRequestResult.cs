namespace MarketAnalyzer.Abstractions
{
    public class PaymentRequestResult
    {
        /// <summary>
        /// موفق/ناموفق
        /// </summary>
        public required bool Succeeded { get; init; }
        /// <summary>
        /// کد پاسخ
        /// </summary>
        public required int ResponseCode { get; init; }
        /// <summary>
        /// شناسه درخواست
        /// </summary>
        public required string RequestId { get; init; }
        /// <summary>
        /// شرح خطا
        /// </summary>
        public required string Error { get; init; }
    }
}
