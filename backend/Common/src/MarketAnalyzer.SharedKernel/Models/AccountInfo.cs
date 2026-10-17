namespace MarketAnalyzer.SharedKernel
{
    public class AccountInfo
    {
        /// <summary>
        /// نام حساب
        /// </summary>
        public required string Name { get; init; }
        /// <summary>
        /// کد حساب
        /// </summary>
        public required string Code { get; init; }
        /// <summary>
        /// سطح حساب
        /// </summary>
        public required AccountEntryLevel Level { get; init; }
    }
}
