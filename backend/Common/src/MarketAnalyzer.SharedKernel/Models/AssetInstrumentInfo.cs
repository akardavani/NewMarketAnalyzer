using System;

namespace MarketAnalyzer.SharedKernel
{
    public class AssetInstrumentInfo
    {
        public required string InstrumentId { get; init; }
        
        /// <summary>
        /// نام 30 رقمي فارسي شرکت (LSoc30)
        /// </summary>
        public required string CompanyName { get; init; }
        /// <summary>
        /// کد 18 رقمي فارسي نماد (LVal18AFC)
        /// </summary>
        public required string NativeName { get; init; }
        /// <summary>
        /// کد 12 رقمی شرکت (CIsin)
        /// </summary>
        public required string CompanyIsin { get; init; }
        /// <summary>
        /// نام 30 رقمي فارسي نماد (LVal30)
        /// </summary>
        public required string FullName { get; init; }
        /// <summary>
        /// نام 18 رقمي لاتين نماد (LVal18)
        /// </summary>
        public required string Name { get; init; }
        /// <summary>
        /// شناسه نماد پایه
        /// </summary>
        public required string BaseInstrumentId { get; init; }
        /// <summary>
        /// نوع نماد 
        /// </summary>
        public required int InstrumentTypeId { get; init; }
        /// <summary>
        /// نام مرتب شده‌ی فارسی نماد
        /// </summary>
        public string NormalizedNativeName { get; init; }
        public required string InstrumentTypeName { get; init; }
        /// <summary>
        /// کد حساب نوع نماد
        /// </summary>
        public required string InstrumentTypeDetailAccountCode { get; init; }
        /// <summary>
        /// بازار
        /// </summary>
        public required int ExchangeId { get; init; }
        /// <summary>
        /// 
        /// </summary>
        public required int? IssuingPurpose { get; init; }
        public required string CounterpartInstrumentId { get; init; }

        /// <summary>
        /// اطلاعات تکمیلی قرارداد اختیار
        /// </summary>
        public required AssetOptionContract OptionContract { get; init; }
        /// <summary>
        /// اطلاعات تکمیلی اوراق
        /// </summary>
        public required AssetBond? Bond { get; init; }
        /// <summary>
        /// اطلاعات تکمیلی حق تقدم
        /// </summary>
        public required AssetRightContract? RightContract { get; init; }
        /// <summary>
        /// اطلاعات تکمیلی قرارداد آتی
        /// </summary>
        public required AssetFutureContract? FutureContract { get; init; }

        /// <summary>
        /// سهام، حق تقدم و صندوق
        /// </summary>
        public required bool IsStock { get; init; }
        /// <summary>
        /// اوراق
        /// </summary>
        public required bool IsBond { get; init; }
        /// <summary>
        /// حق تقدم
        /// </summary>
        public required bool IsRight { get; init; }
        /// <summary>
        /// اختیار
        /// </summary>
        public required bool IsOption { get; init; }
        /// <summary>
        /// تبعی
        /// </summary>
        public required bool IsEmbedded { get; init; }
        /// <summary>
        /// اختیار خرید تبعی
        /// </summary>        
        public required bool IsEmbeddedCallOption { get; init; }
        /// <summary>
        /// اختیار فروش تبعی
        /// </summary> 
        public required bool IsEmbeddedPutOption { get; init; }
        //public required AssetInstrumentInfo? BaseInstrument { get; init; }
    }

    /// <summary>
    /// اطلاعات تکمیلی قرارداد اختیار
    /// </summary>
    public class AssetOptionContract
    {
        /// <summary>
        /// اندازه قرارداد
        /// </summary>
        public long Size { get; init; }
        /// <summary>
        /// قیمت اعمال
        /// </summary>
        public decimal ExercisePrice { get; init; }
        /// <summary>
        /// تاریخ شروع قرارداد
        /// </summary>
        public DateOnly StartDate { get; init; }
        /// <summary>
        /// تاریخ پایان قرارداد
        /// </summary>
        public DateOnly EndDate { get; init; }
        /// <summary>
        /// تاریخ اعمال
        /// </summary>
        public DateOnly ExerciseDate { get; init; }
    }

    /// <summary>
    /// اطلاعات تکمیلی اوراق
    /// </summary>
    public class AssetBond
    {
        /// <summary>
        /// دوره پرداخت سود
        /// </summary>
        public int? PaymentInterval { get; init; }
        /// <summary>
        /// تاریخ انتشار
        /// </summary>
        public DateOnly IssueDate { get; init; }
        /// <summary>
        /// تاریخ سر رسید
        /// </summary>
        public DateOnly MatureDate { get; init; }
        /// <summary>
        /// مبلغ اسمی
        /// </summary>
        public decimal NominalPrice { get; init; }
        /// <summary>
        /// نرخ سود
        /// </summary>
        public decimal InterestRate { get; init; }
    }

    /// <summary>
    /// اطلاعات تکمیلی حق تقدم
    /// </summary>
    public class AssetRightContract
    {
        /// <summary>
        /// قیمت اسمی
        /// </summary>
        public decimal NominalPrice { get; init; }
        /// <summary>
        /// تاریخ شروع
        /// </summary>
        public DateOnly? StartDate { get; init; }
        /// <summary>
        /// تاریخ پایان
        /// </summary>
        public DateOnly? EndDate { get; init; }
    }

    /// <summary>
    /// اطلاعات تکمیلی قرارداد آتی
    /// </summary>
    public class AssetFutureContract
    {        
        /// <summary>
        /// اندازه قرارداد
        /// </summary>
        public int Quantity { get; init; }
        /// <summary>
        /// تاریخ شروع قرارداد
        /// </summary>
        public DateOnly? StratDate { get; init; }
        /// <summary>
        /// تاریخ پایان قرارداد 
        /// </summary>
        public DateOnly EndDate { get; init; }        
    }
}