using System;

namespace MarketAnalyzer.ApiClient.Models
{
    public class FundStatusReturnResult
    {
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateOnly Date { get; init; }

        /// <summary>
        /// کل واحدهای صندوق
        /// </summary>
        public long UnitsCount { get; init; }
        /// <summary>
        /// عمر صندوق
        /// </summary>
        public long FundDuration { get; init; }
        /// <summary>
        /// صدور NAV
        /// </summary>
        public decimal IssuanceNav { get; init; }
        /// <summary>
        /// ابطال NAV
        /// </summary>
        public decimal CancellationNav { get; init; }

        /// <summary>
        /// ابطال ماه گذشته NAV 
        /// </summary>
        public decimal PreviousMonthCancellationNav { get; init; }
        /// <summary>
        /// ابطال هفته گذشته NAV 
        /// </summary>
        public decimal PreviousWeekCancellationNav { get; init; }
        /// <summary>
        /// تعداد واحد صادر شده
        /// </summary>
        public long IssuedUnitsCount { get; init; }
        /// <summary>
        /// تعداد واحد باطل شده
        /// </summary>
        public long CancelledUnitsCount { get; init; }
        /// <summary>
        /// ارزش واحدهای صادر شده
        /// </summary>
        public decimal ValueOfIssuedUnits { get; init; }
        /// <summary>
        /// ارزش واحدهای ابطال شده
        /// </summary>
        public decimal ValueOfCancellationUnits { get; init; }


        /// <summary>
        /// بازدهی روزانه صندوق	
        /// </summary>
        public decimal DailyFundReturn { get; init; }
        /// <summary>
        /// بازدهی هفته اخیر
        /// </summary>
        public decimal WeeklyFundReturn { get; init; }
        /// <summary>
        /// بازدهی ماه اخیر
        /// </summary>
        public decimal MonthlyFundReturn { get; init; }
        /// <summary>
        /// بازدهی ابتدای فعالیت
        /// </summary>
        public decimal ReturnSinceInception { get; init; }
        /// <summary>
        /// تعداد سرمایه گذاران حقیقی
        /// </summary>
        public long NaturalInvestorsCount { get; init; }
        /// <summary>
        /// تعداد سرمایه گذاران حقوقی
        /// </summary>
        public long LegalInvestorsCount { get; init; }
        /// <summary>
        /// درصد دارایی از کل سرمایه گذاران حقیقی
        /// </summary>
        public decimal NaturalOwnershipPercentage { get; init; }
        /// <summary>
        /// درصد دارایی از کل سرمایه گذاران حقوقی
        /// </summary>
        public decimal LegalOwnershipPercentage { get; init; }
        /// <summary>
        /// ارزش (نقد و شبه نقد)
        /// </summary>
        public decimal ValueOfCash { get; init; }
        /// <summary>
        /// درصد دارایی از کل نقد و شبه نقد
        /// </summary>
        public decimal CashOwnershipPercentage { get; init; }
        /// <summary>
        /// ارزش سهام
        /// </summary>
        public decimal ValueOfStock { get; init; }
        /// <summary>
        /// درصد دارایی از کل سهام
        /// </summary>
        public decimal StockOwnershipPercentage { get; init; }
    }
}
