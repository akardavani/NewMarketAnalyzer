using System;

namespace MarketAnalyzer.DataProvider.FipiranModels
{
    public class MutualFundLicense
    {
        /// <summary>
        /// شناسه مجوز
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// شماره مجوز
        /// </summary>
        public string RegNo { get; set; }
        /// <summary>
        /// منقضی شده
        /// </summary>
        public bool IsExpired { get; set; }
        /// <summary>
        /// تاریخ شروع اعتبار
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// تاریخ پایان اعتبار
        /// </summary>
        public DateTime? ExpireDate { get; set; }
        /// <summary>
        /// شماره مجوز
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// شناسه وضعیت مجوز
        /// </summary>
        public int LicenseStatusId { get; set; }
        /// <summary>
        /// وضعیت مجوز
        /// </summary>
        public string LicenseStatusDescription { get; set; }
        /// <summary>
        /// نوع مجوز
        /// </summary>
        public int LicenseTypeId { get; set; }
        /// <summary>
        /// شناسه نوع مجوز جدید
        /// </summary>
        public int? NewLicenseTypeId { get; set; }
    }
}
