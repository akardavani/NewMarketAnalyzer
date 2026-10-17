using System;

namespace MarketAnalyzer.DataProvider.MabnaModels
{
    public class Eps : EntityBase
    {
        /// <summary>
        /// شرکت
        /// </summary>
        public Company Company { get; set; }

        /// <summary>
        /// مجمع
        /// </summary>
        public Meeting Meeting { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AnnouncementType AnnouncementType { get; set; }

        /// <summary>
        /// تاریخ اعلام EPS
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// سال مالی
        /// </summary>
        public DateTime FiscalYear { get; set; }

        /// <summary>
        /// دوره مالی اطلاعات EPS (در حالت ارائه اطلاعات میاندوره‌ای)
        /// </summary>
        public int FiscalPeriod { get; set; }

        /// <summary>
        /// اطلاعات حسابرسی شده است
        /// </summary>
        public bool IsAudited { get; set; }

        /// <summary>
        /// اطلاعات تلفیقی است
        /// </summary>
        public bool IsCombined { get; set; }

        /// <summary>
        /// اطلاعات تجدید ارائه شده است
        /// </summary>
        public bool IsRepresented { get; set; }

        /// <summary>
        /// سرمایه
        /// </summary>
        public long Capital { get; set; }

        /// <summary>
        /// سود ناخالص تحقق یافته (در حالت مجمع)
        /// </summary>
        public int ImpureRealizedEps { get; set; }

        /// <summary>
        /// سود ناخالص تحقق یافته (در حالت مجمع)
        /// </summary>
        public int PureRealizedEps { get; set; }

        /// <summary>
        /// سود ناخالص نقدی توزیع شده (در حالت مجمع)
        /// </summary>
        public int ImpureDps { get; set; }

        /// <summary>
        /// سود خالص نقدی توزیع شده (در حالت مجمع)
        /// </summary>
        public int PureDps { get; set; }

        /// <summary>
        /// سود ناخالص پیش بینی شده (در حالت پیش بینی)
        /// </summary>
        public int ImpureForecastEps { get; set; }

        /// <summary>
        /// سود خالص پیش بینی شده (در حالت پیش بینی)
        /// </summary>
        public int PureForecastEps { get; set; }

        /// <summary>
        /// شناسه شرکت
        /// </summary>
        public string CompanyId => Company?.Id;
        /// <summary>
        /// شناسه مجمع
        /// </summary>
        public string MeetingId => Meeting?.Id;
    }
}
