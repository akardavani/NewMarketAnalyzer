using System;

namespace MarketAnalyzer.DataProvider.MabnaModels
{
    public class CapitalChange : EntityBase
    {
        /// <summary>
        /// شرکت
        /// </summary>
        public Company Company { get; set; }
        /// <summary>
        /// تاریخ افزایش سرمایه
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// مجمع
        /// </summary>
        public Meeting Meeting { get; set; }
        /// <summary>
        /// سرمایه شرکت قبل از افزایش سرمایه(ریال)
        /// </summary>
        public long PreviousCapital { get; set; }
        /// <summary>
        /// سرمایه شرکت پس از افرایش سرمایه (ریال)
        /// </summary>
        public long NewCapital { get; set; }
        /// <summary>
        /// درصد افزایش سرمایه از محل مطالبات و آورده نقدی سهامداران
        /// </summary>
        public decimal ContributionPercent { get; set; }
        /// <summary>
        /// درصد افزایش سرمایه از محل اندوخته‌ها
        /// </summary>
        public decimal ReservePercent { get; set; }
        /// <summary>
        /// درصد افزایش سرمایه از محل صرف سهام
        /// </summary>
        public decimal PremiumPercent { get; set; }
        /// <summary>
        /// تاریخ پایان پذیره نویسی
        /// </summary>
        public DateTime? UnderwritingEndDate { get; set; }
        /// <summary>
        /// تاریخ ثبت افزایش سرمایه
        /// </summary>
        public DateTime RegistrationDate { get; set; }
        /// <summary>
        /// تاریخ دریافت برگه سهام
        /// </summary>
        public DateTime? StockCertificateReceiveDate { get; set; }
        /// <summary>
        /// تاریخ فروش حق تقدم
        /// </summary>
        public DateTime? WarrantSellDate { get; set; }

        /// <summary>
        /// شناسه شرکت
        /// </summary>
        public string CompanyId => Company?.Id;
    }
}
