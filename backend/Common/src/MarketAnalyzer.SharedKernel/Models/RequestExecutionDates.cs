using System;

namespace MarketAnalyzer.SharedKernel
{
    /// <summary>
    /// اطلاعات تاریخ اجرای درخواست‌های صدور/ابطال
    /// </summary>
    public class RequestExecutionDates
    {
        /// <summary>
        /// پایه NAV تاریخ
        /// </summary>
        public required DateOnly BaseNavDate { get; init; }
        /// <summary>
        /// تاریخ انجام
        /// </summary>
        public required DateOnly ExecuteDate { get; init; }
    }
}
