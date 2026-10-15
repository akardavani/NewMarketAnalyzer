namespace MarketAnalyzer.SharedKernel
{
    /// <summary>
    /// نوع حساب بانکی
    /// </summary>
    public enum BankAccountTypeKernel
    {
        /// <summary>
        /// جاری
        /// </summary>
        Transaction = 1,
        /// <summary>
        /// پس انداز
        /// </summary>
        Saving = 2,
        /// <summary>
        /// سپرده کوتاه مدت
        /// </summary>
        ShortTimeDeposit = 3,
        /// <summary>
        /// LongTimeDeposit
        /// </summary>
        LongTimeDeposit = 4
    }
}
