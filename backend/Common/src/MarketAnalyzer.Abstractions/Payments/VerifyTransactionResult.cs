namespace MarketAnalyzer.Abstractions
{
    /// <summary>
    /// نتیجه‌ی اعتبارسنجی
    /// </summary>
    public class VerifyTransactionResult
    {
        /// <summary>
        /// موفق/ناموفق
        /// </summary>
        public required bool Succeeded { get; init; }
        /// <summary>
        /// کد وضعیت
        /// </summary>
        public required int StatusCode { get; init; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public required string Description { get; init; }
    }
}
