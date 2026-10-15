namespace MarketAnalyzer.SharedKernel
{
    public class AccountBalanceSummary
    {
        /// <summary>
        /// مانده حساب
        /// </summary>
        public required decimal Balance { get; init; }
        /// <summary>
        /// کد حساب معین
        /// </summary>
        public required string SubsidiaryAccountCode { get; init; }
        /// <summary>
        /// کد حساب تفصیلی سطح 1
        /// </summary>
        public required string Level1DetailAccountCode { get; init; }
        /// <summary>
        /// کد حساب تفصیلی سطح 2
        /// </summary>
        public required string Level2DetailAccountCode { get; init; }
        /// <summary>
        /// کد حساب تفصیلی سطح 3
        /// </summary>
        public required string Level3DetailAccountCode { get; init; }
        /// <summary>
        /// کد حساب تفصیلی سطح 4
        /// </summary>
        public required string Level4DetailAccountCode { get; init; }
        /// <summary>
        /// کد حساب تفصیلی سطح 5
        /// </summary>
        public required string Level5DetailAccountCode { get; init; }
    }
}
