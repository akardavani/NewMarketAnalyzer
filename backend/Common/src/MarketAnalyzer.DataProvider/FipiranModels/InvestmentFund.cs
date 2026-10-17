using System;
using System.Collections.Generic;

namespace MarketAnalyzer.DataProvider.FipiranModels
{
    public class InvestmentFund
    {
        /// <summary>
        /// شماره ثبت نزد سازمان
        /// </summary>
        public string RegNo { get; set; }

        /// <summary>
        /// نام صندوق
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// تاریخ آغاز فعالیت
        /// </summary>
        public DateTime InitiationDate { get; set; }

        /// <summary>
        /// نوع صندوق
        /// </summary>
        public int FundType { get; set; }

        /// <summary>
        /// آدرس لینک اساسنامه
        /// </summary>
        public string ArticlesOfAssociationLink { get; set; }

        /// <summary>
        /// آدرس لینک امیدنامه
        /// </summary>
        public string ProsoectusLink { get; set; }

        ///// <summary>
        ///// تاریخ
        ///// </summary>
        //public DateTime Date { get; set; }

        /// <summary>
        /// دوره تقسیم سود
        /// </summary>
        public int? DividendIntervalPeriod { get; set; }

        /// <summary>
        /// نرخ سود پیشبینی شده
        /// </summary>
        public decimal? EstimatedEarningRate { get; set; }

        /// <summary>
        /// نرخ سود تضمینی
        /// </summary>
        public decimal? GuaranteedEarningRate { get; set; }

        /// <summary>
        /// تعداد واحدهای سرمایه گذاری شده
        /// </summary>
        public long? InvestedUnits { get; set; }

        /// <summary>
        /// شیوه‌ی سرمایه گذاری
        /// </summary>
        public string TypeOfInvest { get; set; }

        /// <summary>
        /// مدیر
        /// </summary>
        public string Manager { get; set; }

        /// <summary>
        /// شماره ثبت مدیر نزد سازمان
        /// </summary>
        public string ManagerSeoRegisterNo { get; set; }

        /// <summary>
        /// شماره ثبت ضامن نقدشوندگی نزد سازمان
        /// </summary>
        public string GuarantorSeoRegisterNo { get; set; }

        /// <summary>
        /// حسابرس
        /// </summary>
        public string Auditor { get; set; }

        /// <summary>
        /// آدرس وب سایتها
        /// </summary>
        public List<string> WebsiteAddress { get; set; }

        /// <summary>
        /// متولی
        /// </summary>
        public string Custodian { get; set; }

        /// <summary>
        /// ضامن نقدشوندگی
        /// </summary>
        public string Guarantor { get; set; }

        public decimal? Beta { get; set; }

        public decimal? Alpha { get; set; }

        public bool IsCompleted { get; set; }

        /// <summary>
        /// شناسه یکتای نماد - فقط صندوقهای قابل معامله
        /// </summary>
        public string InsCode { get; set; }

        /// <summary>
        /// نماد - فقط صندوقهای قابل معامله
        /// </summary>
        public string SmallSymbolName { get; set; }

        /// <summary>
        /// ناشر
        /// </summary>
        public int FundPublisher { get; set; }
    }
}
