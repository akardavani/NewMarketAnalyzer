using System;
using System.Collections.Generic;

namespace MarketAnalyzer.SharedKernel
{
    public class CreateVoucherRequest
    {
        /// <summary>
        /// شناسه یکتای درخواست برای تشخیص هنگام ثبت سند گروهی
        /// </summary>
        public required Guid Id { get; init; }
        /// <summary>
        /// شاخص نوع سند
        /// </summary>
        public required VoucherTypeIndicator VoucherType { get; init; }
        /// <summary>
        /// تاریخ سند
        /// </summary>
        public required DateOnly Date { get; init; }
        /// <summary>
        /// شرح سند
        /// </summary>
        public required string Description { get; init; }
        /// <summary>
        /// ردیف‌های سند
        /// </summary>
        public required IReadOnlyCollection<AutoVoucherItem> Items { get; init; }
    }
}