namespace MarketAnalyzer.Abstractions
{
    /// <summary>
    /// وضعیت آیتم تراکنش گروهی
    /// </summary>
    public enum BatchTransferItemState
    {
        /// <summary>
        /// تایید شده
        /// </summary>
        Confirmed,
        /// <summary>
        /// تایید نشده
        /// </summary>
        NotConfirmed,
        /// <summary>
        /// عودت ذینفع
        /// </summary>
        BeneficiaryReturned,
        /// <summary>
        /// در انتظار تائید شعبه
        /// </summary>
        AwaitingBranchApproval,
        /// <summary>
        /// عدم تایید شعبه
        /// </summary>
        BranchNotApproved,
        /// <summary>
        /// در انتظار تایید مالی
        /// </summary>
        AwaitingFinancialApproval,
        /// <summary>
        /// عدم تایید مالی
        /// </summary>
        FinancialNotApproved,
        /// <summary>
        /// عودت حواله ساتنا
        /// </summary>
        SatnaReturn,
        /// <summary>
        /// عودت شده
        /// </summary>
        Returned
    }
}