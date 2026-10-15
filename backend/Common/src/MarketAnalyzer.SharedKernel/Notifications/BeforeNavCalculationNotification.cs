using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.SharedKernel.Notifications
{
    /// <summary>
    /// رویداد قبل از شروع محاسبه خالص ارزش دارایی‌ها
    /// </summary>
    public class BeforeNavCalculationNotification : INotification
    {
        /// <summary>
        /// NAV تاریخ محاسبه
        /// </summary>
        public required DateOnly NavDate { get; init; }
        /// <summary>
        /// NAV تعداد واحدهای صندوق در آخرین 
        /// </summary>
        public required long LastUnitsCount { get; init; }
        /// <summary>
        /// NAV تعداد درخواستهای صدور آخرین 
        /// </summary>
        public required long IssuanceRequestsCount { get; init; }
        /// <summary>
        /// NAV تعداد درخواستهای ابطال آخرین 
        /// </summary>
        public required long CancellationRequestsCount { get; init; }
        /// <summary>
        /// NAV تعداد سرمایه گذاران در آخرین
        /// </summary>
        public required long LastNavInvestorsCount { get; init; }
        /// <summary>
        /// آخرین خالص ارزش داراییها
        /// </summary>
        public required decimal LastAssetsValue { get; init; }
        /// <summary>
        /// تاریخ آخرین روز کاری 
        /// </summary>
        public required DateOnly WorkingDay { get; init; }
        /// <summary>
        // ارزش دارایی روز "کاری" قبل
        /// </summary>
        public required decimal PreviousWorkingDayAssetsValue { get; init; }
        /// <summary>
        /// ابطال NAV آخرین
        /// </summary>
        public required decimal LastCancellationNav { get; init; }
        /// <summary>
        /// ابطال NAV سود و زیان آخرین  
        /// </summary>
        public required decimal LastTotalIncome { get; init; }        
        /// <summary>
        /// صدور NAV آخرین
        /// </summary>
        public required decimal LastIssuanceNav { get; init; }
        /// <summary>
        /// کد حساب تفصیلی مدیر
        /// </summary>
        public required string ManagerDetailAccountCode { get; init; }
    }
}
