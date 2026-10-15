namespace MarketAnalyzer.Sejam.Models
{
    public enum SejamBankAccountType
    {
        None,
        /// <summary>
        /// بلند مدت
        /// </summary>
        LongTermAccount,
        /// <summary>
        /// کوتاه مدت
        /// </summary>
        ShortTermAccount,
        /// <summary>
        /// جاری
        /// </summary>
        CurrentAccount,
        /// <summary>
        /// قرض الحسنه
        /// </summary>
        SavingAccount
    }
}