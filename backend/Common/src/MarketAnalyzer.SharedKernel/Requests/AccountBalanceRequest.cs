using System;

namespace MarketAnalyzer.SharedKernel
{
    public class AccountBalanceRequest
    {
        /// <summary>
        /// شناسه درخواست
        /// </summary>
        public Guid RequestId { get; } = Guid.NewGuid();

        /// <summary>
        /// کد حساب
        /// </summary>
        public required AccountValue Account { get; init; }
        /// <summary>
        /// از تاریخ
        /// </summary>
        public DateOnly? FromDate { get; init; }
        /// <summary>
        /// تا تاریخ
        /// </summary>
        public required DateOnly? ToDate { get; init; }
        /// <summary>
        /// کد حساب تفصیلی سطح 1
        /// </summary>
        public string Level1DetailAccountCode { get; init; }
        /// <summary>
        /// کد حساب تفصیلی سطح 2
        /// </summary>
        public string Level2DetailAccountCode { get; init; }
        /// <summary>
        /// کد حساب تفصیلی سطح 3
        /// </summary>
        public string Level3DetailAccountCode { get; init; }
        /// <summary>
        /// کد حساب تفصیلی سطح 4
        /// </summary>
        public string Level4DetailAccountCode { get; init; }
        /// <summary>
        /// کد حساب تفصیلی سطح 5
        /// </summary>
        public string Level5DetailAccountCode { get; init; }
    }
}
