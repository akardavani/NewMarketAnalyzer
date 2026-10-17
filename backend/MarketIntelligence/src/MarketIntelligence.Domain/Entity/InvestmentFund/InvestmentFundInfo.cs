namespace MarketIntelligence.Domain
{
    public class InvestmentFundInfo: IEntity
    {
        public string Id { get; set; }
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

        //public long FundSize { get; set; }

        /// <summary>
        /// نوع صندوق
        /// </summary>
        public int FundType { get; set; }

        /// <summary>
        /// مدیر اجرایی
        /// </summary>
        public string ExecutiveManager { get; set; }

        /// <summary>
        /// آدرس لینک اساسنامه
        /// </summary>
        public string ArticlesOfAssociationLink { get; set; }

        /// <summary>
        /// آدرس لینک امیدنامه
        /// </summary>
        public string ProsoectusLink { get; set; }

        /// <summary>
        /// تاریخ به روز رسانی
        /// </summary>
        public DateTime LastModificationTime { get; set; }

        /// <summary>
        /// تاریخ
        /// </summary>
        public DateTime Date { get; set; }

        //public float dailyEfficiency { get; set; }
        //public float weeklyEfficiency { get; set; }
        //public float monthlyEfficiency { get; set; }
        //public float quarterlyEfficiency { get; set; }
        //public float sixMonthEfficiency { get; set; }
        //public float annualEfficiency { get; set; }

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

        //public int insInvNo { get; set; }
        //public float insInvPercent { get; set; }
        //public float legalPercent { get; set; }

        /// <summary>
        /// بازارگردان
        /// </summary>
        public string MarketMaker { get; set; }

        //public float naturalPercent { get; set; }

        /// <summary>
        /// خالص ارزش داراییها
        /// </summary>
        public decimal NetAsset { get; set; }

        //public int retInvNo { get; set; }
        //public float retInvPercent { get; set; }

        /// <summary>
        /// تعداد واحدهای سرمایه گذاری شده
        /// </summary>
        public long InvestedUnits { get; set; }

        /// <summary>
        /// تعداد واحدهای ابطال شده در روز
        /// </summary>
        public long UnitsRedDAY { get; set; }

        /// <summary>
        /// تعداد واحدهای ابطال شده از ابتدای صندوق
        /// </summary>
        public long UnitsRedFromFirst { get; set; }

        /// <summary>
        /// تعداد واحدهای صادر شده در روز
        /// </summary>
        public long UnitsSubDAY { get; set; }

        /// <summary>
        /// تعداد واحدهای صادر شده از ابتدا
        /// </summary>
        public long UnitsSubFromFirst { get; set; }

        //public float efficiency { get; set; }

        /// <summary>
        /// ابطال NAV
        /// </summary>
        public decimal CancelNav { get; set; }

        /// <summary>
        /// صدور NAV
        /// </summary>
        public decimal IssueNav { get; set; }

        /// <summary>
        /// آماری NAV
        /// </summary>
        public decimal StatisticalNav { get; set; }

        //public float fiveBest { get; set; }
        //public float stock { get; set; }
        //public float bond { get; set; }
        //public float other { get; set; }
        //public float cash { get; set; }
        //public float deposit { get; set; }
        //public object fundUnit { get; set; }
        //public object commodity { get; set; }

        /// <summary>
        /// شیوه‌ی سرمایه گذاری
        /// </summary>
        public string TypeOfInvest { get; set; }

        //public object rankOf12Month { get; set; }
        //public object rankOf24Month { get; set; }
        //public object rankOf36Month { get; set; }
        //public object rankOf48Month { get; set; }
        //public object rankOf60Month { get; set; }
        //public DateTime rankLastUpdate { get; set; }

        /// <summary>
        /// مدیر
        /// </summary>
        public string Manager { get; set; }

        /// <summary>
        /// شماره ثبت مدیر نزد سازمان
        /// </summary>
        public string ManagerSeoRegisterNo { get; set; }

        /// <summary>
        /// ضمان نقدشوندگی
        /// </summary>
        public string Guarantor { get; set; }

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
        /// مدیر سرمایه گذاری
        /// </summary>
        public string InvestmentManager { get; set; }

        public decimal? Beta { get; set; }

        public decimal? Alpha { get; set; }

        //public object fundWatch { get; set; }

        /// <summary>
        /// تاریخ ثبت نزد سازمان
        /// </summary>
        public DateTime? SeoRegisterDate { get; set; }

        /// <summary>
        /// شماره ثبت نزد اداره ثبت شرکتها
        /// </summary>
        public string RegistrationNumber { get; set; }

        /// <summary>
        /// تاریخ ثبت نزد اداره ثبت شرکتها
        /// </summary>
        public DateTime? RegisterDate { get; set; }

        /// <summary>
        /// شناسه ملی
        /// </summary>
        public string NationalId { get; set; }

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
