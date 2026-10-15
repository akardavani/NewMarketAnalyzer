using System;

namespace MarketAnalyzer.SharedKernel
{
    public class FinancialStatementRequest
    {
        /// <summary>
        /// تا تاریخ
        /// </summary>
        public required DateOnly? ToDate { get; init; }
    }
}
