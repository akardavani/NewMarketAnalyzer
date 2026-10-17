using System;

namespace MarketAnalyzer.SharedKernel
{
    public class AccountTurnoverProjection
    {
        /// <summary>
        /// تاریخ شروع سال مالی
        /// </summary>
        public required DateOnly FiscalYearStartDate { get; init; }
        /// <summary>
        /// تاریخ پایان سال مالی
        /// </summary>
        public required DateOnly FiscalYearEndDate { get; init; }
        /// <summary>
        /// شناسه سیستمی سند
        /// </summary>
        public required long VoucherId { get; init; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public required DateOnly VoucherDate { get; init; }
        /// <summary>
        /// شرح سند
        /// </summary>
        public required string VoucherDescription { get; init; }
        /// <summary>
        /// شرح ردیف
        /// </summary>
        public required string ItemDescription { get; init; }
        /// <summary>
        /// گردش بدهکار
        /// </summary>
        public required decimal Debit { get; init; }
        /// <summary>
        /// گردش بستانکار
        /// </summary>
        public required decimal Credit { get; init; }
        /// <summary>
        /// شناسه نوع سند
        /// </summary>
        public required int VoucherTypeId { get; init; }
        /// <summary>
        /// اندیکاتور نوع سند
        /// </summary>
        public required int? VoucherTypeIndicator { get; init; }
        /// <summary>
        /// حساب گروه ماهیت بدهکار دارد
        /// </summary>
        public required bool GroupAccountIsDebitNature { get; init; }
        /// <summary>
        /// حساب کل ماهیت بدهکار دارد
        /// </summary>
        public required bool LedgerAccountIsDebitNature { get; init; }
        /// <summary>
        /// حساب معین ماهیت بدهکار دارد
        /// </summary>
        public required bool SubsidiaryAccountIsDebitNature { get; init; }
        /// <summary>
        /// کد گروه
        /// </summary>
        public required string GroupAccountCode { get; init; }
        /// <summary>
        /// کد حساب کل
        /// </summary>
        public required string LedgerAccountCode { get; init; }
        /// <summary>
        /// کد حساب معین
        /// </summary>
        public required string SubsidiaryAccountCode { get; init; }
        /// <summary>
        /// شاخص حساب معین
        /// </summary>
        public required int? SubsidiaryAccountIndicator { get; init; }
        /// <summary>
        /// کد حساب تفصیلی سطح 1
        /// </summary>
        public required string Level1DetailAccountCode { get; init; }
        /// <summary>
        /// عنوان حساب تفصیلی سطح 1
        /// </summary>
        public required string Level1DetailAccountTitle { get; init; }
        /// <summary>
        /// کد حساب تفصیلی سطح 2
        /// </summary>
        public required string Level2DetailAccountCode { get; init; }
        /// <summary>
        /// عنوان حساب تفصیلی سطح 2
        /// </summary>
        public required string Level2DetailAccountTitle { get; init; }
        /// <summary>
        /// کد حساب تفصیلی سطح 3
        /// </summary>
        public required string Level3DetailAccountCode { get; init; }
        /// <summary>
        /// عنوان حساب تفصیلی سطح 3
        /// </summary>
        public required string Level3DetailAccountTitle { get; init; }
        /// <summary>
        /// کد حساب تفصیلی سطح 4
        /// </summary>
        public required string Level4DetailAccountCode { get; init; }
        /// <summary>
        /// عنوان حساب تفصیلی سطح 4
        /// </summary>
        public required string Level4DetailAccountTitle { get; init; }
        /// <summary>
        /// کد حساب تفصیلی سطح 5
        /// </summary>
        public required string Level5DetailAccountCode { get; init; }
        /// <summary>
        /// عنوان حساب تفصیلی سطح 5
        /// </summary>
        public required string Level5DetailAccountTitle { get; init; }
    }
}
