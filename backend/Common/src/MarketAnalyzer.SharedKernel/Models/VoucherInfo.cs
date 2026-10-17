using System;

namespace MarketAnalyzer.SharedKernel
{
    /// <summary>
    /// مشخصات سند ایجاد شده توسط دامنه‌ی حسابداری
    /// </summary>
    public class VoucherInfo
    {
        /// <summary>
        /// شناسه سند
        /// </summary>
        public required long VoucherId { get; init; }
        /// <summary>
        /// شماره سند
        /// </summary>
        public required long VoucherNumber { get; init; }
        /// <summary>
        /// شناسه درخواست
        /// </summary>
        public required Guid RequestId { get; init; }
    }
}