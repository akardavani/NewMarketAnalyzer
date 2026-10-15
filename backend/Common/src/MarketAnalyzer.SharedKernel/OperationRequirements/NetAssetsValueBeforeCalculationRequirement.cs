using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.SharedKernel
{
    /// <summary>
    /// الزامات قبل از محاسبه خالص ارزش دارایی
    /// </summary>
    public class NetAssetsValueBeforeCalculationRequirement : IOperationRequirement
    {
        /// <summary>
        /// نام عملیات
        /// </summary>
        public required string Name { get; init; }
        /// <summary>
        /// تاریخ محاسبه
        /// </summary>
        public required DateOnly Date { get; init; }
    }
}
