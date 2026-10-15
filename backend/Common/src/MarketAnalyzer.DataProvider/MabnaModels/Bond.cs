using MarketAnalyzer.DataProvider.Json;
using System;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.DataProvider.MabnaModels
{
    /// <summary>
    /// اوراق مشارکت
    /// </summary>
    public class Bond : EntityBase
    {
        /// <summary>
        /// نام اوراق مشارکت
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// نام انگلیسی اوراق مشارکت
        /// </summary>
        public string EnglishName { get; set; }
        /// <summary>
        /// نام کوتاه اوراق مشارکت
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// نام کوتاه انگلیسی اوراق مشارکت
        /// </summary>
        public string EnglishShortName { get; set; }
        /// <summary>
        /// تاریخ انتشار
        /// </summary>
        public DateTime IssueDate { get; set; }
        /// <summary>
        /// تاریخ سر رسید
        /// </summary>
        public DateTime MatureDate { get; set; }
        /// <summary>
        /// تاریخ شروع پذیره نویسی
        /// </summary>
        public DateTime? UnderwritingStartDate { get; set; }
        /// <summary>
        /// تاریخ پایان پذیره نویسی
        /// </summary>
        public DateTime? UnderwritingEndDate { get; set; }
        /// <summary>
        /// مبلغ اسمی
        /// </summary>
        public decimal NominalPrice { get; set; }
        /// <summary>
        /// نرخ سود
        /// </summary>
        public decimal InterestRate { get; set; }
        /// <summary>
        /// نرخ سود ابطالی
        /// </summary>
        public decimal? CancellationInterestRate { get; set; }
        /// <summary>
        /// دوره پرداخت سود
        /// </summary>
        public PaymentInterval PaymentInterval { get; set; }
        /// <summary>
        /// هزینه نگهداری
        /// </summary>
        public decimal? MaintenanceCost { get; set; }
        /// <summary>
        /// نماد معاملاتی
        /// </summary>
        public string TradeSymbol { get; set; }
        /// <summary>
        /// نماد معاملاتی انگلیسی
        /// </summary>
        public string EnglishTradeSymbol { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// تعداد اوراق منتشر شده
        /// </summary>
        public int? TotalBonds { get; set; }
        /// <summary>
        /// کد دسته بندی
        /// </summary>
        public string CategoryCode { get; set; }
        /// <summary>
        /// مبلغ نهایی فروش رفته / تخصیص داده شده
        /// </summary>
        public decimal? FinalSellPrice { get; set; }
        /// <summary>
        /// نرخ سود قطعی
        /// </summary>
        public decimal? RealizedRate { get; set; }
        /// <summary>
        /// نرخ سود اصلاحی
        /// </summary>
        public decimal? AdjustedRate { get; set; }
        /// <summary>
        /// تاریخ مجوز انتشار
        /// </summary>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? OfferingCertificateDate { get; set; }
        /// <summary>
        /// ماده قانونی (برای اوراق دولتی)
        /// </summary>
        public string LegislationDescription { get; set; }
        /// <summary>
        /// مصوبه هیئت وزیران
        /// </summary>
        public string MinistryApprovalDescription { get; set; }
        /// <summary>
        /// دارایی مبنای انتشار اوراق
        /// </summary>
        public string GuarantyDescription { get; set; }
        /// <summary>
        /// مرجع صدور مجوز
        /// </summary>
        public string CertificateAuthority { get; set; }
    }
}
