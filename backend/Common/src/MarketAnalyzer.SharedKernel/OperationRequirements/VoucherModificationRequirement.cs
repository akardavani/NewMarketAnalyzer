using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.SharedKernel
{
    public class VoucherModificationRequirement : IOperationRequirement
    {
        /// <summary>
        /// نام عملیات
        /// </summary>
        public required string Name { get; init; }
        /// <summary>
        /// تاریخ سند
        /// </summary>
        public required DateOnly Date { get; init; }
    }
}
