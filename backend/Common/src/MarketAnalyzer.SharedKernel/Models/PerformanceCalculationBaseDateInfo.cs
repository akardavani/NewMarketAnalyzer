using System;

namespace MarketAnalyzer.SharedKernel
{
    public class PerformanceCalculationBaseDateInfo
    {
        /// <summary>
        /// تاریخ شروع
        /// </summary>
        public required DateOnly StartDate { get; init; }
        /// <summary>
        /// تاریخ پایان
        /// </summary>
        public required DateOnly EndDate { get; init; }
        /// <summary>
        /// تاریخ مبنای محاسبات
        /// </summary>
        public required DateOnly BaseDate { get; init; }
    }
}
