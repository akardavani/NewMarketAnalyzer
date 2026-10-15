namespace MarketAnalyzer.Abstractions
{
    public enum AccountStatus
    {
        /// <summary>
        /// نامشخص
        /// </summary>
        None = 0,

        /// <summary>
        /// بسته شده
        /// </summary>
        Closed = 1,

        /// <summary>
        /// فعال
        /// </summary>
        Active = 2,

        /// <summary>
        /// مسدود با قابلیت واریز
        /// </summary>
        BlockedDepositAllowed = 3,

        /// <summary>
        /// مسدود بدون قابلیت واریز
        /// </summary>
        BlockedNoDeposit = 4,

        /// <summary>
        /// راکد
        /// </summary>
        Dormant = 5,

        /// <summary>
        /// خطا در پاسخ دهی
        /// </summary>
        ErrorResponse = 6,

        /// <summary>
        /// سایر موارد
        /// </summary>
        Others = 7,

        /// <summary>
        /// حساب پیدا نشد
        /// </summary>
        NotFound = 8
    }
}
