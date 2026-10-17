namespace MarketAnalyzer.SharedKernel
{
    public static class ReportsName
    {
        #region [Assets]

        /// <summary>
        /// فایل گواهی ها
        /// </summary>
        public const string Certificate = "Assets/Certificate_Report";

        /// <summary>
        /// فایل اعلامیه صدور
        /// </summary>
        public const string IssuanceAnnouncement = "Assets/IssuanceAnnouncement_Report";

        /// <summary>
        /// فایل اعلامیه ابطال
        /// </summary>
        public const string CancellationAnnouncement = "Assets/CancellationAnnouncement_Report";

        /// <summary>
        /// فایل جزئیات مدیر بر اساس عملکرد
        /// </summary>
        public const string PerformanceBasedManagerDetail = "Assets/PerformanceBasedManagerDetail_Report";

        /// <summary>
        /// فایل درخواست‌ها
        /// </summary>
        public const string Requests = "Assets/Requests_Report";

        /// <summary>
        /// فایل navها
        /// </summary>
        public const string NetAssetValues = "Assets/NetAssetValues_Report";
        /// <summary>
        /// فایل وضعیت صندوق
        /// </summary>
        public const string FundStatus = "Assets/FundStatus_Report";

        /// <summary>
        /// فایل لیست navها
        /// </summary>
        public const string NetAssetValuesList = "Assets/NetAssetValuesList_Report";

        /// <summary>
        /// فایل کاردکس درخواست‌های سرمایه‌گذار
        /// </summary>
        public const string RequestCardexes = "Assets/RequestCardexes_Report";

        /// <summary>
        /// فایل هزینه‌های ثابت
        /// </summary>
        public const string FixedCosts = "Assets/FixedCosts_Report";

        /// <summary>
        /// فایل نوع هزینه‌های ثابت
        /// </summary>
        public const string FixedCostTypes = "Assets/FixedCostTypes_Report";

        /// <summary>
        /// فایل درخواست‌های وثیقه
        /// </summary>
        public const string CollateralRequests = "Assets/CollateralRequests_Report";

        /// <summary>
        /// فایل سود پرداختنی به سرمایه‌گذار
        /// </summary>
        public const string DailyInterestPayableToInvestors = "Assets/DailyInterestPayableToInvestors_Report";

        /// <summary>
        /// فایل جزئیات کارمزد ارکان
        /// </summary>
        public const string PillarPerformanceBasedDetails = "Assets/PillarPerformanceBasedDetails_Report";


        #endregion

        #region [Bourse]

        /// <summary>
        /// فایل قالب تعدیل قیمت
        /// </summary>
        public const string PriceAdjustmentTemplate = "Bourse/PriceAdjustmentTemplate";

        /// <summary>
        /// فایل لیست تعدیل قیمت
        /// </summary>
        public const string PriceAdjustments = "Bourse/PriceAdjustments_Report";

        /// <summary>
        /// فایل لیست معاملات
        /// </summary>
        public const string Trades = "Bourse/Trades_Report";

        /// <summary>
        /// فایل عملکرد سبد
        /// </summary>
        public const string Portfolios = "Bourse/Portfolios_Report";

        /// <summary>
        /// فایل کاردکس سهام
        /// </summary>
        public const string Cardices = "Bourse/Cardices_Report";
        /// <summary>
        /// فایل کاردکس اوراق غیر بورسی
        /// </summary>
        public const string BankCertificateDepositCardices = "Bourse/BankCertificateDepositCardices_Report";

        /// <summary>
        /// فایل عملکرد اوراق با درآمد ثابت
        /// </summary>
        public const string BondPortfolios = "Bourse/BondPortfolios_Report";

        /// <summary>
        /// لیست اوراق با درآمد ثابت
        /// </summary>
        public const string BondsList = "Bourse/BondsList_Report";

        /// <summary>
        /// فایل اختیار معامله
        /// </summary>
        public const string OptionsPortfolio = "Bourse/OptionsPortfolio_Report";

        /// <summary>
        /// فایل تبعی
        /// </summary>
        public const string EmbeddedsPortfolio = "Bourse/EmbeddedsPortfolio_Report";

        /// <summary>
        /// فایل دریافت سود سهام
        /// </summary>
        public const string Dividends = "Bourse/Dividends_Report";

        /// <summary>
        /// فایل افزایش سرمایه
        /// </summary>
        public const string CapitalChanges = "Bourse/CapitalChanges_Report";

        /// <summary>
        /// فایل نمادها
        /// </summary>
        public const string Instruments = "Bourse/Instruments_Report";

        /// <summary>
        /// فایل افشاس ماهانه پورتفو
        /// </summary>
        public const string PortfolioStatement = "Bourse/PortfolioStatement_Report";

        /// <summary>
        /// فایل صورت مالی
        /// </summary>
        public const string FinancialStatement = "Bourse/FinancialStatement_Report";

        /// <summary>
        /// فایل سود و زیان فروش
        /// </summary>
        public const string SalesProfitLoss = "Bourse/SalesProfitLoss_Report";

        /// <summary>
        /// فایل سود و زیان اعمال اختیار
        /// </summary>
        public const string OptionSettlementProfitLoss = "Bourse/OptionSettlementProfitLoss_Report";

        /// <summary>
        /// فایل سود و زیان تحقق نیافته
        /// </summary>
        public const string UnrealizedProfitLoss = "Bourse/UnrealizedProfitLoss_Report";

        /// <summary>
        /// فایل دارایی ها به تفکیک صنعت
        /// </summary>
        public const string AssetsBySectors = "Bourse/AssetsBySectors_Report";

        /// <summary>
        /// فایل دارایی ها به تفکیک صنعت
        /// </summary>
        public const string BankCertificateDepositTrades = "Bourse/BankCertificateDepositTrades_Report";
        #endregion

        #region [Accounting]

        /// <summary>
        /// فایل اسناد
        /// </summary>
        public const string Vouchers = "Accounting/Vouchers_Report";

        /// <summary>
        /// فایل ترازنامه به ارزش روز
        /// </summary>
        public const string FairValueBalanceSheet = "Accounting/FairValueBalanceSheet_Report";

        /// <summary>
        /// فایل تراز آزمایشی
        /// </summary>
        public const string TrialBalance = "Accounting/TrialBalance_Report";

        /// <summary>
        /// فایل گزارش سود و زیان
        /// </summary>
        public const string IncomeStatement = "Accounting/IncomeStatement_Report";

        /// <summary>
        /// فایل ترازنامه
        /// </summary>
        public const string BalanceSheet = "Accounting/BalanceSheet_Report";

        /// <summary>
        /// فایل جزئیات سند
        /// </summary>
        public const string VoucherInfo = "Accounting/VoucherInfo_Report";

        /// <summary>
        /// فایل دفتر حساب
        /// </summary>
        public const string AccountingBook = "Accounting/AccountingBook_Report";

        /// <summary>
        /// فایل دفتر روزنامه
        /// </summary>
        public const string JournalBook = "Accounting/JournalBook_Report";

        /// <summary>
        /// فایل مدیریت حسابها
        /// </summary>
        public const string Accounts = "Accounting/Accounts_Report";


        #endregion

        #region [Cash]

        public const string OfflinePaymentOrderState = "Cash/OfflinePaymentOrderState";
        /// <summary>
        /// فایل گزارش لیست دستور پرداخت
        /// </summary>
        public const string PaymentOrders = "Cash/PaymentOrders_Report";

        #endregion
    }
}