using System.Collections.Generic;

namespace MarketAnalyzer.SharedKernel
{
    public class AutoVoucherItem
    {
        /// <summary>
        /// شرح
        /// </summary>
        public required string Description { get; init; }
        /// <summary>
        /// بدهکار
        /// </summary>
        public required decimal Debit { get; init; }
        /// <summary>
        /// بستانکار
        /// </summary>
        public required decimal Credit { get; init; }
        /// <summary>
        /// شاخص یا کد حساب معین
        /// </summary>
        public required AccountValue Account { get; init; }
        /// <summary>
        /// کد حسابهای تفصیلی
        /// </summary>
        public required IReadOnlyCollection<AutoVoucherItemDetailAccount> DetailAccounts { get; init; }
    }
}