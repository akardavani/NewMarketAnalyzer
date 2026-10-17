namespace MarketAnalyzer.Abstractions
{
    public class TransactionResult
    {
        /// <summary>
        /// موفق/ناموفق
        /// </summary>
        public required bool Succeeded { get; init; }
        /// <summary>
        /// شناسه تراکنش که بعد از پرداخت بانک به ما میدهد
        /// </summary>
        public required string TransactionId { get; init; }
        /// <summary>
        /// کد وضعیت
        /// </summary>
        public required int StatusCode { get; init; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public required string Description { get; init; }
        /// <summary>
        /// اطلاعات کامل تراکنش بصورت JSON
        /// </summary>
        public required string TransctionData { get; init; }
    }
}
