using System;

namespace MarketAnalyzer.SharedKernel
{
    /// <summary>
    /// اطلاعات سال مالی
    /// </summary>
    public class FiscalYearInfo
    {
        /// <summary>
        /// شناسه سال مالی
        /// </summary>
        public required int Id { get; init; }
        /// <summary>
        /// عنوان سال مالی
        /// </summary>
        public required string Title { get; init; }
        /// <summary>
        /// تاریخ شروع
        /// </summary>
        public required DateOnly StartDate { get; init; }
        /// <summary>
        /// تاریخ پایان
        /// </summary>
        public required DateOnly EndDate { get; init; }
        /// <summary>
        /// فعال/غیرفعال
        /// </summary>
        public required bool Active { get; init; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public required string Description { get; init; }
    }
}
