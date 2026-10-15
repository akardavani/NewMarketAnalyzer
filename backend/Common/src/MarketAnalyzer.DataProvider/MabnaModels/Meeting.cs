using System;

namespace MarketAnalyzer.DataProvider.MabnaModels
{
    public class Meeting : EntityBase
    {
        /// <summary>
        /// شرکت
        /// </summary>
        public Company Company { get; set; }

        /// <summary>
        /// نوع مجمع
        /// </summary>
        public MeetingType Type { get; set; }

        /// <summary>
        /// تاریخ مجمع
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// سال مالی
        /// </summary>
        public DateTime FiscalYear { get; set; }
    }
}
