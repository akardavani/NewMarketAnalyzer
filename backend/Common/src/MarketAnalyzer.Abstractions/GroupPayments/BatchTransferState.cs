namespace MarketAnalyzer.Abstractions
{
    /// <summary>
    /// وضعیت کلی تراکنش گروهی
    /// </summary>
    public enum BatchTransferState
    {
        /// <summary>
        /// ثبت شده
        /// </summary>
        Entry,
        /// <summary>
        /// انجام شده
        /// </summary>
        Executed,
        /// <summary>
        /// 
        /// </summary>
        Failed,
        /// <summary>
        /// 
        /// </summary>
        AwaitingOwnersApproval,
        /// <summary>
        /// برگشت خورده
        /// </summary>
        RejectedByOwners
    }
}