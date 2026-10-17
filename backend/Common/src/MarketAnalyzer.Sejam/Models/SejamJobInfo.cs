using System;

namespace MarketAnalyzer.Sejam.Models
{
    public class SejamJobInfo
    {
        /// <summary>
        /// تاریخ اشتغال
        /// </summary>
        public DateTime? EmploymentDate { get; set; }
        /// <summary>
        /// نام نهاد / سازمان محل کار
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// نشانی محل کار
        /// </summary>
        public string CompanyAddress { get; set; }
        /// <summary>
        /// کد پستی محل کار
        /// </summary>
        public string CompanyPostalCode { get; set; }
        /// <summary>
        /// پست الکترونیکی محل کار
        /// </summary>
        public string CompanyEmail { get; set; }
        /// <summary>
        /// تارنمای محل کار
        /// </summary>
        public string CompanyWebSite { get; set; }
        /// <summary>
        /// پیش شماره شهر محل کار
        /// </summary>
        public string CompanyCityPrefix { get; set; }
        /// <summary>
        /// شماره تلفن ثابت محل کار
        /// </summary>
        public string CompanyPhone { get; set; }
        /// <summary>
        /// توضیحات شغل
        /// </summary>
        public string JobDescription { get; set; }
        /// <summary>
        /// سمت
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// پیش شماره دور نگار محل کار
        /// </summary>
        public string CompanyFaxPrefix { get; set; }
        /// <summary>
        /// شماره دور نگار محل کار
        /// </summary>
        public string CompanyFax { get; set; }
        /// <summary>
        /// عنوان شغل
        /// </summary>
        public SejamEntry Job { get; set; }
    }
}