using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.SharedKernel
{
    public class FiscalYearCreatedNotification : INotification
    {
        /// <summary>
        /// تاریخ شروع سال مالی
        /// </summary>
        public required DateOnly StartDate { get; init; }
        /// <summary>
        /// تاریخ پایان سال مالی
        /// </summary>
        public required DateOnly EndDate { get; init; }
        /// <summary>
        /// تاریخ پایان سال مالی قبلی
        /// </summary>
        public required DateOnly PreviousFiscalYearEndDate { get; init; }
        /// <summary>
        /// شناسه اپراتور
        /// </summary>
        public required int OperatorId { get; init; }
    }
}

