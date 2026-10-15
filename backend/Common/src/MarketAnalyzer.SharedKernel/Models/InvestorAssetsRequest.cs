using System;
using System.Collections.Generic;

namespace MarketAnalyzer.SharedKernel
{
    public class InvestorAssetsRequest
    {
        /// <summary>
        /// تاریخ
        /// </summary>
        public required DateOnly ToDate { get; init; }
        /// <summary>
        /// نوع شخص
        /// </summary>
        public required int? PersonTypeId { get; init; }
        /// <summary>
        /// شناسه بانک
        /// </summary>
        public required int? BankId { get; init; }
        /// <summary>
        /// کلمه کلیدی برای جستجو در کد ملی/نام شخص
        /// </summary>
        public required string Keyword { get; init; }
        /// <summary>
        /// کد حساب تفصیلی برای جستجو
        /// </summary>
        public required IReadOnlyCollection<string> DetailAccountsCodes { get; init; }
        /// <summary>
        /// محاسبه مبلغ قابل پرداخت برای درخواستهای ابطالی که اجرا نشده‌ااند.
        /// </summary>
        public required bool IncludeCancellationRequestsPayableAmount { get; init; }
    }
}
