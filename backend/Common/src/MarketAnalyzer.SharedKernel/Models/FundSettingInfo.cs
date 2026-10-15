using System;

namespace MarketAnalyzer.SharedKernel
{
    public class FundSettingInfo
    {
        /// <summary>
        /// سقف تعدیل قیمت سهام
        /// </summary>
        public required decimal MaxStockPriceAdjustment { get; init; }
        /// <summary>
        /// نرخ مالیات
        /// </summary>
        public required decimal VatFee { get; init; }
        /// <summary>
        /// نرخ عوارض
        /// </summary>
        public required decimal TollFee { get; init; }
        /// <summary>
        /// ارزش مبنا
        /// </summary>
        public required decimal NominalValue { get; init; }
        /// <summary>
        /// کارمزد ثابت صدور
        /// </summary>
        public required decimal FixedIssuanceFee { get; init; }
        /// <summary>
        /// کارمزد ثابت ابطال
        /// </summary>
        public required decimal FixedCancellationFee { get; init; }
        /// <summary>
        /// تاریخ شروع فعالیت
        /// </summary>
        public required DateOnly ActivityStartDate { get; init; }
        /// <summary>
        /// ضریب متغیر کارمزد صدور
        /// </summary>
        public required decimal IssuanceFeeVariableCoefficient { get; init; }
        /// <summary>
        ///  سقف کارمزد متغیر صدور
        /// </summary>
        public required decimal MaxIssuanceFeeVariable { get; init; }
        /// <summary>
        /// حداقل تعداد واحد هر درخواست
        /// </summary>
        public required long MinRequestUnits { get; init; }
        /// <summary>
        /// تعداد واحد ممتاز
        /// </summary>
        public required long SuperUnitsCount { get; init; }
        /// <summary>
        /// نرخ تنزیل سود سهام
        /// </summary>
        public required decimal DividendDiscountRate { get; init; }
        /// <summary>
        /// ضریب سالانه کارمزد تصفیه صندوق
        /// </summary>
        public required decimal SettlementFeeCoefficient { get; init; }
        /// <summary>
        /// مقدار ریالی کارمزد تصفیه صندوق
        /// </summary>
        public decimal SettlementFee { get; init; }
        /// <summary>
        /// در نظر گرفتن مالیات و عوارض برای تصفیه صندوق
        /// </summary>
        public required bool ApplySettlementTax { get; init; }
        /// <summary>
        /// در نظر گرفتن سقف ریالی کارمزد تصفیه
        /// </summary>
        public required bool ConsiderMaxForSettleWage { get; init; }
        /// <summary>
        /// حداکثر خالص ارزش صندوق برای تصفیه
        /// </summary>
        public required decimal MaxNavForSettlement { get; init; }
        /// <summary>
        /// مبلغ ریالی تصفیه از خالص ارزش دارایی
        /// </summary>
        public required decimal MaxSettlementAmount { get; init; }
        /// <summary>
        /// مبلغ خالص ارزش صندوق مازاد بر سقف قبلی
        /// </summary>
        public required decimal MaxSettlementExtraAmount { get; init; }
        /// <summary>
        /// محاسبه ذخیره تغییر ارزش سهام
        /// </summary>
        public required bool CalculateStockValueChanges { get; init; }
        /// <summary>
        /// نرخ بازدهی تضمین/پیشبینی شده
        /// </summary>
        public required decimal EfficiencyOrForecastRate { get; init; }
        /// <summary>
        /// قیمت اسمی حق تقدم
        /// </summary>
        public required decimal RightNominalPrice { get; init; }
    }
}
