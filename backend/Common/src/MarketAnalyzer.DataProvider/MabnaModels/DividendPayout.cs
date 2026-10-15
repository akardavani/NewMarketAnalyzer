using System;

namespace MarketAnalyzer.DataProvider.MabnaModels
{
    public class DividendPayout : EntityBase
    {
        /// <summary>
        /// مجمع
        /// </summary>
        public Meeting Meeting { get; set; }
        /// <summary>
        /// تاریخ پرداخت
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// تاریخ اعلام
        /// </summary>
        public DateTime AnnouncementDate { get; set; }

        /// <summary>
        /// شناسه مجمع
        /// </summary>
        public string MeetingId => Meeting?.Id;
        /// <summary>
        /// شناسه شرکت
        /// </summary>
        public string CompanyId => Meeting?.Company?.Id;
    }
}
