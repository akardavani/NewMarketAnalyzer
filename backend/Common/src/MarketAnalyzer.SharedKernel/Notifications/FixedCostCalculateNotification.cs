using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.SharedKernel
{
    public class FixedCostCalculateNotification : INotification
    {
        /// <summary>
        /// NAV تاریخ محاسبه 
        /// </summary>
        public required DateOnly NavDate { get; init; }
        /// <summary>
        /// شناسه اپراتور
        /// </summary>
        public required int OperatorId { get; init; }
    }
}
