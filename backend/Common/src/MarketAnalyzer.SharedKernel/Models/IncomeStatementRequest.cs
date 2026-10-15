using System;

namespace MarketAnalyzer.SharedKernel
{
    public class IncomeStatementRequest
    {
        /// <summary>
        /// از تاریخ
        /// </summary>
        public required DateOnly FromDate { get; init; }
        /// <summary>
        /// تا تاریخ
        /// </summary>
        public required DateOnly ToDate { get; init; }
        /// <summary>
        /// به تفکیک حساب
        /// </summary>
        public required AccountEntryLevel AccountLevel { get; init; }
    }
}
