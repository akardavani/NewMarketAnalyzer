namespace MarketAnalyzer.SharedKernel
{
    /// <summary>
    /// سطح حساب
    /// </summary>
    public enum AccountEntryLevel
    {
        /// <summary>
        /// گروه
        /// </summary>
        Group = 1,
        /// <summary>
        /// کل
        /// </summary>
        Ledger = 2,
        /// <summary>
        /// معین
        /// </summary>
        Subsidiary = 3,
        /// <summary>
        /// معین سطح 1
        /// </summary>
        Level1Subsidiary = 4,
        /// <summary>
        /// تفصیلی
        /// </summary>
        Detail = 5
    }
}
