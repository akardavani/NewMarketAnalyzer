namespace MarketAnalyzer.SharedKernel
{
    public record InvestorAssets
    {
        /// <summary>
        /// شناسه سرمایه گذار
        /// </summary>
        public required long InvestorId { get; init; }
        /// <summary>
        /// نام سرمایه گذار
        /// </summary>
        public required string InvestorName { get; init; }
        /// <summary>
        /// کد ملی سرمایه گذار
        /// </summary>
        public required string NationalCode { get; init; }
        /// <summary>
        /// کد حساب تفصیلی
        /// </summary>
        public required string DetailAccountCode { get; init; }
        /// <summary>
        /// نام بانک
        /// </summary>
        public required string BankName { get; init; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public required string BankBranchName { get; init; }
        /// <summary>
        /// شماره حساب
        /// </summary>
        public required string AccountNumber { get; init; }
        /// <summary>
        /// شماره شبا
        /// </summary>
        public required string ShebaNumber { get; init; }
        /// <summary>
        /// تعداد واحدها
        /// </summary>
        public required long Assets { get; init; }
        /// <summary>
        /// صدورهای در انتظار
        /// </summary>
        public required int PendingIssuanceRequests { get; init; }
        /// <summary>
        /// مبلغ قابل پرداخت برای درخواست‌های ابطال تایید شده که اجرا نشده‌اند
        /// </summary>
        public required decimal CancellationRequestsPayableAmount { get; init; }
    }
}
