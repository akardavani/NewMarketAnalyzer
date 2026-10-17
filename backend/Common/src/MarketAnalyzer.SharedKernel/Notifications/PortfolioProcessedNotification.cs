using System;
using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.SharedKernel.Notifications
{
    /// <summary>
    /// رویداد اتمام پردازش سبد سهام
    /// </summary>
    public class PortfolioProcessedNotification : INotification
    {
        /// <summary>
        /// Nav تاریخ محاسبه 
        /// </summary>
        public required DateOnly CurrentNavDate { get; init; }
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
        /// NAV خالص ارزش داراییها در آخرین 
        /// </summary>
        public required decimal LastAssetsValue { get; init; }
        /// <summary>
        /// تاریخ آخرین روز کاری 
        /// </summary>
        public required DateOnly WorkingDay { get; init; }
        /// <summary>
        /// ارزش دارایی روز کاری قبل
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
        /// ارزش سهام خریداری شده
        /// </summary>
        public required decimal PurchasedSharesValue { get; init; }
        /// <summary>
        /// ارزش سهام فروخته شده
        /// </summary>
        public required decimal SoldSharesValue { get; init; }
        /// <summary>
        /// سرمایه گذاری بدون تعدیل قیمت
        /// </summary>
        public required decimal NoAdjastedInvestment { get; init; }
        /// <summary>
        /// دارای سهامی با قیمت تعدیل شده هستیم
        /// <para>آماری استفاده کنیم تا مغایرت نخوریم NAV اگر نداشتیم کافیه همان مبلغ سرمایه گذاری‌ها از صورت مالی را برای محاسبه</para>
        /// </summary>
        public required bool HasPriceAdjustment { get; init; }
        /// <summary>
        /// کارمزد خرید سهام پرتفو
        /// </summary>
        public required decimal BuyCommission { get; init; }
        /// <summary>
        /// کارمزد فروش سهام پرتفو - تعدیل شده
        /// </summary>
        public required decimal SellCommission { get; init; }
        ///// <summary>
        ///// کد حساب تفصیلی مدیر صندوق
        ///// <para>برای محاسبه کارمزد مدیر از سرمایه گذاری در سپرده بانکی</para>
        ///// </summary>
        public required string ManagerDetailAccountCode { get; init; }
    }
}
