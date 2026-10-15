using System;

namespace MarketAnalyzer.SharedKernel
{
    public class IndexHistoryInfo
    {
        /// <summary>
        /// تاریخ مورد نظر
        /// </summary>
        public required DateOnly DateIndex { get; init; }
        /// <summary>
        /// مقدار شاخص 
        /// </summary>
        public required decimal IndexValue { get; init; }
    }
}
