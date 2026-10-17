using System;

namespace MarketAnalyzer.DataProvider.MabnaModels
{
    /// <summary>
    /// پرداخت سود اوراق مشارکت
    /// </summary>
    public class BondInterestPayment : EntityBase
    {
        /// <summary>
        /// تاریخ شروع
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// تاریخ پایان
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// سود هر سهم
        /// </summary>
        public decimal Interest { get; set; }
        /// <summary>
        /// اوراق مشارکت
        /// </summary>
        public Bond Bond { get; set; }

        /// <summary>
        /// شناسه اوراق مشارکت
        /// </summary>
        public string BondId => Bond?.Id;
    }
}
