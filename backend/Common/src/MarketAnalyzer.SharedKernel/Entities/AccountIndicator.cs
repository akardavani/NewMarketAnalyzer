using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.SharedKernel
{
    public class AccountIndicator : Enumeration
    {
        #region [Groups]

        /// <summary>
        /// دارایی های جاری
        /// 01
        /// </summary>
        public static readonly AccountIndicator CurrentAssets = new(1, "دارایی های جاری");
        /// <summary>
        /// بدهی های جاری
        /// 02
        /// </summary>
        public static readonly AccountIndicator Liabilities = new(2, "بدهی های جاری");
        /// <summary>
        /// خالص دارایی ها
        /// 03
        /// </summary>
        public static readonly AccountIndicator NetAssets = new(3, "خالص دارایی ها");
        /// <summary>
        /// درآمد
        /// 04
        /// </summary>
        public static readonly AccountIndicator Revenue = new(4, "درآمد");
        /// <summary>
        /// هزینه
        /// 05
        /// </summary>
        public static readonly AccountIndicator Costs = new(5, "هزینه");
        /// <summary>
        /// حسابهای انتظامی
        /// 09
        /// </summary>
        public static readonly AccountIndicator MemorandumAccounts = new(6, "حسابهای انتظامی");

        #endregion

        #region [Ledgers]

        /// <summary>
        /// نقد و بانک
        /// 0111
        /// </summary>
        public static readonly AccountIndicator BankAndCash = new(100, "نقد و بانک");
        /// <summary>
        /// سرمایه گذاری در سپرده بانکی و گواهی سپرده بانکی
        /// 0115
        /// </summary>
        public static readonly AccountIndicator Ledger1 = new(101, "سرمایه گذاری در سپرده بانکی و گواهی سپرده بانکی");
        /// <summary>
        /// سرمایه گذاری در اوراق بهادار با درآمد ثابت
        /// 0116
        /// </summary>
        public static readonly AccountIndicator Ledger2 = new(102, "سرمایه گذاری در اوراق بهادار با درآمد ثابت");
        /// <summary>
        /// سرمایه گذاری در سهام و حق تقدم شرکتهای بورسی
        /// 0117
        /// </summary>
        public static readonly AccountIndicator Ledger3 = new(103, "سرمایه گذاری در سهام و حق تقدم شرکتهای بورسی");
        /// <summary>
        /// درآمد تعهد پذیره نویسی اوراق
        /// 0446
        /// </summary>
        public static readonly AccountIndicator Ledger4 = new(104, "درآمد تعهد پذیره نویسی اوراق");
        /// <summary>
        /// خالص ارزش دارایی ها
        /// 0331
        /// </summary>
        public static readonly AccountIndicator Ledger5 = new(105, "خالص دارایی ها");
        /// <summary>
        /// تعدیلات
        /// 0332
        /// </summary>
        public static readonly AccountIndicator Adjustments = new(106, "تعدیلات");
        /// <summary>
        /// سود (زیان) انباشته 
        /// 0357
        /// </summary>
        public static readonly AccountIndicator RetainedEarnings = new(107, "سود (زیان) انباشته");
        /// <summary>
        /// حسابهای دریافتنی تجاری 
        /// 0112
        /// </summary>
        public static readonly AccountIndicator TradeAccountsReceivables = new(108, "حسابهای دریافتنی تجاری");
        /// <summary>
        /// جاری کارگزاران 
        /// 0113
        /// </summary>
        public static readonly AccountIndicator Brokers = new(109, "جاری کارگزاران");
        /// <summary>
        /// سایر حساب های دریافتنی 
        /// 0114
        /// </summary>
        public static readonly AccountIndicator OtherAccountsReceivables = new(110, "سایر حساب های دریافتنی");
        /// <summary>
        /// مخارج انتقالی به دورههای آتی 
        /// 0118
        /// </summary>
        public static readonly AccountIndicator TransferredCostsToFuture = new(111, "مخارج انتقالی به دورههای آتی");
        /// <summary>
        /// سایر حسابهای پرداختنی
        /// 0225
        /// </summary>
        public static readonly AccountIndicator OtherAccountsPayable = new(112, "سایر حسابهای پرداختنی");
        /// <summary>
        /// بدهی به سرمایه گذاران
        /// 0222
        /// </summary>
        public static readonly AccountIndicator DebtToInvestors = new(113, "بدهی به سرمایه گذاران");
        /// <summary>
        /// پرداختنی به ارکان صندوق
        /// 0223
        /// </summary>
        public static readonly AccountIndicator PayableToPillars = new(114, "پرداختنی به ارکان صندوق");
        /// <summary>
        /// ذخیره هزینه ارکان
        /// 0224
        /// </summary>
        public static readonly AccountIndicator PillarCostReserve = new(115, "ذخیره هزینه ارکان");
        /// <summary>
        /// سرمایه گذاری در صندوق و سایر اوراق
        /// 0119
        /// </summary>
        public static readonly AccountIndicator FundInvestment = new(116, "سرمایه گذاری در صندوق و سایر اوراق");
        /// <summary>
        /// سود (زیان) فروش اوراق بهادار
        /// 0441
        /// </summary>
        public static readonly AccountIndicator StocksSoldRevenues = new(117, "سود(زیان) فروش اوراق بهادار");
        /// <summary>
        /// درآمد سود سهام
        /// 0442
        /// </summary>
        public static readonly AccountIndicator DividendIncomes = new(118, "درآمد سود سهام");
        /// <summary>
        /// سود سپرده سرمایه گذاری و گواهی سپرده بانکی
        /// 0443
        /// </summary>
        public static readonly AccountIndicator BankDepositInterests = new(119, "سود سپرده سرمایه گذاری و گواهی سپرده بانکی");
        /// <summary>
        /// سود اوراق بهادار با درآمد ثابت
        /// 0444
        /// </summary>
        public static readonly AccountIndicator BondsInterests = new(120, "سود اوراق بهادار با درآمد ثابت");
        /// <summary>
        /// درآمد (هزینه) ناشی از تغییر ارزش سرمایه گذاری ها
        /// 0445
        /// </summary>
        public static readonly AccountIndicator StockValueChangesIncomes = new(121, "درآمد (هزینه) ناشی از تغییر ارزش سرمایه گذاری ها");
        /// <summary>
        /// سایر درآمدها
        /// 0447
        /// </summary>
        public static readonly AccountIndicator OtherIncomes = new(122, "سایر درآمدها");

        #endregion

        #region  [Subsidiaies]

        /// <summary>
        /// بانک
        /// 011110
        /// </summary>
        public static readonly AccountIndicator Bank = new(200, "بانک");
        /// <summary>
        /// پرداختنی به مدیر (مدیر ثبت) بابت صدور و ابطال
        /// 022350
        /// </summary>
        public static readonly AccountIndicator ManagerPayableForIssuanceAndCancellation = new(201, "پرداختنی به مدیر (مدیر ثبت) بابت صدور و ابطال");
        /// <summary>
        /// بدهی بابت درخواست صدور واحدهای سرمایه گذاری ممتاز
        /// 022210
        /// </summary>
        public static readonly AccountIndicator SuperInvestmentUnitsIssuanceDebt = new(202, "بدهی بابت درخواست صدور واحدهای سرمایه گذاری ممتاز");
        /// <summary>
        /// واحدهای سرمایه گذاری عادی
        /// 033110
        /// </summary>
        public static readonly AccountIndicator OrdinaryInvestmentUnits = new(203, "واحدهای سرمایه گذاری عادی");
        /// <summary>
        /// بدهی بابت درخواست صدور واحدهای سرمایه گذاری عادی
        /// 022220
        /// </summary>
        public static readonly AccountIndicator OrdinaryInvestmentUnitsIssuanceDebt = new(204, "بدهی بابت درخواست صدور واحدهای سرمایه گذاری عادی");
        /// <summary>
        /// تعدیلات ارزشگذاری صدور واحدهای عادی
        /// 033210
        /// </summary>
        public static readonly AccountIndicator OrdinaryInvestmentUnitsValuationAdjustments = new(205, "تعدیلات ارزش گذاری صدور واحدهای عادی");
        /// <summary>
        /// بدهی بابت تتمه واحدهای صادر شده
        /// 022230
        /// </summary>
        public static readonly AccountIndicator IssuedUnitsRemainingDebt = new(206, "بدهی بابت تتمه واحدهای صادر شده");
        /// <summary>
        /// تعدیلات ارزشگذاری ابطال واحدهای عادی
        /// 033220
        /// </summary>
        public static readonly AccountIndicator OrdinaryUnitsCancellationValuationAdjustments = new(207, "تعدیلات ارزش گذاری ابطال واحدهای عادی");
        /// <summary>
        /// بدهی بابت واحدهای ابطال شده
        /// 022240
        /// </summary>
        public static readonly AccountIndicator CanceledUnitsDebt = new(208, "بدهی بابت واحدهای ابطال شده");
        /// <summary>
        /// سرمایه گذاری در سهام
        /// 011710
        /// </summary>
        public static readonly AccountIndicator StocksInvesting = new(209, "سرمایه گذاری در سهام");
        /// <summary>
        /// جاری کارگزار
        /// 011310
        /// </summary>
        public static readonly AccountIndicator Broker = new(210, "جاری کارگزار");
        /// <summary>
        /// تعدیلات ارزشیابی داراییها به خالص ارزش فروش
        /// 033230
        /// </summary>
        public static readonly AccountIndicator AssetsValuationAdjustmentsToNetRealizableValue = new(211, "تعدیلات ارزشیابی داراییها به خالص ارزش فروش");
        /// <summary>
        /// حساب ارزشیابی در سهام
        /// 011720
        /// </summary>
        public static readonly AccountIndicator StocksValuation = new(212, "حساب ارزشیابی در سهام");
        /// <summary>
        /// سرمایه گذاری در حق تقدم سهام
        /// 011730
        /// </summary>
        public static readonly AccountIndicator RightsInvesting = new(213, "سرمایه گذاری در حق تقدم سهام");
        /// <summary>
        /// حساب ارزشیابی در حق تقدم خرید سهام
        /// 011740
        /// </summary>
        public static readonly AccountIndicator RightsValuation = new(214, "حساب ارزشیابی در حق تقدم خرید سهام");
        /// <summary>
        /// سود دریافتنی سهام
        /// 011270
        /// </summary>
        public static readonly AccountIndicator DividendsReceivable = new(215, "سود دریافتنی سهام");
        /// <summary>
        /// درآمد سود سهام
        /// 044210
        /// </summary>
        public static readonly AccountIndicator DividendIncome = new(216, "درآمد سود سهام");
        /// <summary>
        /// حساب تنزیل سود دریافتنی سهام
        /// 055360
        /// </summary>
        public static readonly AccountIndicator DiscountDividendsReceivable = new(217, "حساب تنزیل سود دریافتنی سهام");
        /// <summary>
        /// ذخیره حساب تنزیل سود دریافتنی سهام
        /// 022570
        /// </summary>
        public static readonly AccountIndicator ReserveDiscountDividendsReceivable = new(218, "ذخیره حساب تنزیل سود دریافتنی سهام");
        /// <summary>
        /// درآمد (هزینه) تغییر ارزش سرمایه گذاری در سهام
        /// 044510
        /// </summary>
        public static readonly AccountIndicator StockValueChangesIncome = new(219, "درآمد (هزینه) تغییر ارزش سرمایه گذاری در سهام");
        /// <summary>
        /// هزینه مالیات فروش سهام و حق تقدم
        /// 055610
        /// </summary>
        public static readonly AccountIndicator StocksSoldTax = new(220, "هزینه مالیات فروش سهام و حق تقدم");
        /// <summary>
        /// هزینه کارمزد فروش سهام
        /// 055410
        /// </summary>
        public static readonly AccountIndicator SoldCommission = new(221, "هزینه کارمزد فروش");
        /// <summary>
        /// سود(زیان) ناشی از فروش سهام
        /// 044110
        /// </summary>
        public static readonly AccountIndicator StocksSoldRevenue = new(222, "سود(زیان) ناشی از فروش سهام");
        /// <summary>
        /// سرمایه گذاری در اوراق مشارکت
        /// 011610
        /// </summary>
        public static readonly AccountIndicator BondsInvesting = new(223, "سرمایه گذاری در اوراق مشارکت");
        /// <summary>
        /// سود دریافتنی اوراق مشارکت
        /// 011230
        /// </summary>
        public static readonly AccountIndicator ReceivableBondsInterest = new(224, "سود دریافتنی اوراق مشارکت");
        /// <summary>
        /// سود(زیان) ناشی از فروش گواهی سپرده بانکی
        /// 044140
        /// </summary>
        public static readonly AccountIndicator BankDepositeSoldRevenue = new(225, "سود(زیان) ناشی از فروش گواهی سپرده بانکی");
        /// <summary>
        /// حساب ارزشیابی در اوراق مشارکت
        /// 011620
        /// </summary>
        public static readonly AccountIndicator BondsValuation = new(226, "حساب ارزشیابی در اوراق مشارکت");
        /// <summary>
        /// درآمد تنزیل سود بانک
        /// 044760
        /// </summary>
        public static readonly AccountIndicator BankInterestDiscountIncome = new(227, "درآمد تنزیل سود بانک");
        /// <summary>
        /// درآمد (هزینه) تغییر ارزش سرمایه گذاری در اوراق سلف
        /// 044540
        /// </summary>
        public static readonly AccountIndicator SalafValueChangesIncome = new(229, "درآمد (هزینه) تغییر ارزش سرمایه گذاری در اوراق سلف");
        /// <summary>
        /// درآمد (هزینه) تغییر ارزش سرمایه گذاری در اوراق مشارکت
        /// 044530
        /// </summary>
        public static readonly AccountIndicator BondsValueChangesIncome = new(230, "درآمد (هزینه) تغییر ارزش سرمایه گذاری در اوراق مشارکت");
        /// <summary>
        /// آبونمان نرم افزار صندوق
        /// 011870
        /// </summary>
        public static readonly AccountIndicator SoftwareSubscription = new(231, "آبونمان نرم افزار صندوق");
        /// <summary>
        /// بدهی بابت سود پرداختنی به سرمایه گذاران عادی
        /// 022260
        /// </summary>
        public static readonly AccountIndicator OrdinaryInvestorsPayableInterestDebt = new(232, "بدهی بابت سود پرداختنی به سرمایه گذاران عادی");
        /// <summary>
        /// بدهی بابت سود پرداختنی به سرمایه گذاران ممتاز
        /// 022250
        /// </summary>
        public static readonly AccountIndicator SuperInvestorsPayableInterestDebt = new(233, "بدهی بابت سود پرداختنی به سرمایه گذاران ممتاز");
        /// <summary>
        /// پرداختنی بابت اختیار معامله
        /// 022120
        /// </summary>
        public static readonly AccountIndicator PayableToOptionsTrade = new(234, "پرداختنی بابت اختیار معامله");
        /// <summary>
        /// پرداختنی به حسابرس
        /// 022340
        /// </summary>
        public static readonly AccountIndicator PayableToAuditor = new(235, "پرداختنی به حسابرس");
        /// <summary>
        /// پرداختنی به ضامن
        /// 022330
        /// </summary>
        public static readonly AccountIndicator PayableToGuarantor = new(236, "پرداختنی به ضامن");
        /// <summary>
        /// پرداختنی به متولی
        /// 022320
        /// </summary>
        public static readonly AccountIndicator PayableToTrustee = new(237, "پرداختنی به متولی");
        /// <summary>
        /// پرداختنی به مدیر
        /// 022310
        /// </summary>
        public static readonly AccountIndicator PayableToManager = new(238, "پرداختنی به مدیر");
        /// <summary>
        /// تراز اختتامیه
        /// 099020
        /// </summary>
        public static readonly AccountIndicator ClosingBalance = new(239, "تراز اختتامیه");
        /// <summary>
        /// تراز افتتاحیه
        /// 099010
        /// </summary>
        public static readonly AccountIndicator OpeningBalance = new(240, "تراز افتتاحیه");
        /// <summary>
        /// تسهیلات مالی دریافتی
        /// 022610
        /// </summary>
        public static readonly AccountIndicator ReceivableFinancialFacilities = new(241, "تسهیلات مالی دریافتی");
        /// <summary>
        /// تعدیل کارمزد کارگزار
        /// 044750
        /// </summary>
        public static readonly AccountIndicator BrokerCommissionAdjustment = new(242, "تعدیل کارمزد کارگزار");
        /// <summary>
        /// تغییرات ارزش ناشی از عملکرد
        /// 033120
        /// </summary>
        public static readonly AccountIndicator PerformanceValueChange = new(243, "تغییرات ارزش ناشی از عملکرد");
        /// <summary>
        /// ثبت و نظارت سازمان بورس و اوراق بهادار
        /// 011880
        /// </summary>
        public static readonly AccountIndicator SeoRegistrationAndMonitoring = new(244, "ثبت و نظارت سازمان بورس و اوراق بهادار");
        /// <summary>
        /// جاری كارگزاری از تعدیل معاملات
        /// 011330
        /// </summary>
        public static readonly AccountIndicator BrokerTradesAdjusment = new(245, "جاری كارگزاری از تعدیل معاملات");
        /// <summary>
        /// جاری كارگزاری گواهی سپرده كالایی
        /// 011340
        /// </summary>
        public static readonly AccountIndicator BrokerImeDeposit = new(246, "جاری كارگزاری گواهی سپرده كالایی");
        /// <summary>
        /// جاری کارگزار بابت وجه تضمین قراردادهای آتی موقعیت تعهدی خرید
        /// 011370
        /// </summary>
        public static readonly AccountIndicator BrokerBuyFutureContractMargin = new(247, "جاری کارگزار بابت وجه تضمین قراردادهای آتی موقعیت تعهدی خرید");
        /// <summary>
        /// جاری کارگزار بابت وجه تضمین قراردادهای آتی موقعیت تعهدی فروش
        /// 011360
        /// </summary>
        public static readonly AccountIndicator BrokerSellFutureContractMargin = new(248, "جاری کارگزار بابت وجه تضمین قراردادهای آتی موقعیت تعهدی فروش");
        /// <summary>
        /// چکهای در جریان وصول
        /// 011420
        /// </summary>
        public static readonly AccountIndicator ChequesInProcessOfCollection = new(249, "چکهای در جریان وصول");
        /// <summary>
        /// حسابهای دریافتنی از سرمایه گذاران
        /// 011410
        /// </summary>
        public static readonly AccountIndicator InvestorReceivables = new(251, "حسابهای دریافتنی از سرمایه گذاران");
        /// <summary>
        /// حق پذیرش و عضویت در کانون ها
        /// 055340
        /// </summary>
        public static readonly AccountIndicator CentersAdmissionAndMemberShipRight = new(252, "حق پذیرش و عضویت در کانون ها");
        /// <summary>
        /// خرید اقساطی سهام
        /// 022110
        /// </summary>
        public static readonly AccountIndicator StocksInstallmentPurchase = new(253, "خرید اقساطی سهام");
        /// <summary>
        /// خلاصه حساب سود و زیان
        /// 035710
        /// </summary>
        public static readonly AccountIndicator ProfitAndLossSummary = new(254, "خلاصه حساب سود و زیان");
        /// <summary>
        /// درآمد (هزینه) تغییر ارزش سرمایه گذاری در حق تقدم سهام
        /// 044520
        /// </summary>
        public static readonly AccountIndicator RightValueChangesIncome = new(255, "درآمد (هزینه) تغییر ارزش سرمایه گذاری در حق تقدم سهام");
        /// <summary>
        /// درآمد (هزینه) تغییر ارزش سرمایه گذاری در صندوق های سرمایه گذاری
        /// 044570
        /// </summary>
        public static readonly AccountIndicator FundsValueChangesIncome = new(256, "درآمد (هزینه) تغییر ارزش سرمایه گذاری در صندوق های سرمایه گذاری");
        /// <summary>
        /// درآمد برگشت سود گواهی بانکی
        /// 044720
        /// </summary>
        public static readonly AccountIndicator BankDepositInterestReturnnRevenue = new(259, "درآمد برگشت سود گواهی بانکی");
        /// <summary>
        /// درآمد تعهد پذیره نویسی اوراق بهادار با درآمد ثابت
        /// 044610
        /// </summary>
        public static readonly AccountIndicator FixedIncomeSecuritiesUnderwritingcommitmentRevenue = new(260, "درآمد تعهد پذیره نویسی اوراق بهادار با درآمد ثابت");
        /// <summary>
        /// درآمد سود دریافتنی وجه تضمین نزد سپرده گذاری
        /// 044731
        /// </summary>
        public static readonly AccountIndicator CsdMarginReceivaleInterestRevenue = new(263, "درآمد سود دریافتنی وجه تضمین نزد سپرده گذاری");
        /// <summary>
        /// سود سپرده بانکی
        /// 044320
        /// </summary>
        public static readonly AccountIndicator BankDepositInterest = new(264, "سود سپرده بانکی");
        /// <summary>
        /// سود گواهی سپرده بانکی
        /// 044310
        /// </summary>
        public static readonly AccountIndicator BankDepositCertificateInterest = new(265, "سود گواهی سپرده بانکی");
        /// <summary>
        /// درآمد هزینه ناشی از مبلغ تشویقی فروش تعهدی سهام
        /// 044830
        /// </summary>
        public static readonly AccountIndicator Account73 = new(266, "درآمد هزینه ناشی از مبلغ تشویقی فروش تعهدی سهام");
        /// <summary>
        /// درآمد(هزینه) سرمایه گذاری در قرارداد فروش تعهدی مالک سهام
        /// 044810
        /// </summary>
        public static readonly AccountIndicator Account74 = new(267, "درآمد(هزینه) سرمایه گذاری در قرارداد فروش تعهدی مالک سهام");
        /// <summary>
        /// درآمد(هزینه) سرمایه گذاری در قرارداد فروش تعهدی متقاضی سهام
        /// 044820
        /// </summary>
        public static readonly AccountIndicator Account75 = new(268, "درآمد(هزینه) سرمایه گذاری در قرارداد فروش تعهدی متقاضی سهام");
        /// <summary>
        /// دریافتنی از مالک
        /// 011495
        /// </summary>
        public static readonly AccountIndicator OwnerReceivable = new(269, "دریافتنی از مالک");
        /// <summary>
        /// دریافتنی تعهد پذیره نویسی اوراق بهادار با درآمد ثابت
        /// 011293
        /// </summary>
        public static readonly AccountIndicator Account77 = new(270, "دریافتنی تعهد پذیره نویسی اوراق بهادار با درآمد ثابت");
        /// <summary>
        /// ذخیره برگشت سود گواهی بانکی
        /// 022550
        /// </summary>
        public static readonly AccountIndicator BankCertificateInterestReturnReserve = new(271, "ذخیره برگشت سود گواهی بانکی");
        /// <summary>
        /// ذخیره تبلیغات
        /// 022521
        /// </summary>
        public static readonly AccountIndicator AdvertisingReserve = new(272, "ذخیره تبلیغات");
        /// <summary>
        /// ذخیره تغییر ارزش سهام
        /// 022555
        /// </summary>
        public static readonly AccountIndicator StockChangeValueReserve = new(273, "ذخیره تغییر ارزش سهام");
        /// <summary>
        /// ذخیره تنزیل سود دریافتنی سپرده بانکی
        /// 022560
        /// </summary>
        public static readonly AccountIndicator BankDepositReceivableInterestDiscountReserve = new(274, "ذخیره تنزیل سود دریافتنی سپرده بانکی");
        /// <summary>
        /// ذخیره حق الزحمه حسابرس
        /// 022440
        /// </summary>
        public static readonly AccountIndicator AuditFeesReserve = new(275, "ذخیره حق الزحمه حسابرس");
        /// <summary>
        /// ذخیره کارمزد تصفیه
        /// 022520
        /// </summary>
        public static readonly AccountIndicator SettlementCommissionReserve = new(276, "ذخیره کارمزد تصفیه");
        /// <summary>
        /// ذخیره کارمزد ضامن
        /// 022430
        /// </summary>
        public static readonly AccountIndicator GuarantorCommissionReserve = new(277, "ذخیره کارمزد ضامن");
        /// <summary>
        /// ذخیره کارمزد متولی
        /// 022420
        /// </summary>
        public static readonly AccountIndicator TrusteeCommissionReserve = new(278, "ذخیره کارمزد متولی");
        /// <summary>
        /// ذخیره کارمزد مدیر
        /// 022410
        /// </summary>
        public static readonly AccountIndicator ManagerCommissionReserve = new(279, "ذخیره کارمزد مدیر");
        /// <summary>
        /// ذخیره نگهداری اوراق بهادار بی نام
        /// 022510
        /// </summary>
        public static readonly AccountIndicator Account87 = new(280, "ذخیره نگهداری اوراق بهادار بی نام");
        /// <summary>
        /// ذخیره هزینه نرم افزار
        /// 022540
        /// </summary>
        public static readonly AccountIndicator RezerveSoftwareVariableCost = new(281, "ذخیره هزینه نرم افزار");
        /// <summary>
        /// سایر حسابهای پرداختنی
        /// 022580
        /// </summary>
        public static readonly AccountIndicator Account89 = new(282, "سایر حسابهای پرداختنی");
        /// <summary>
        /// سایر حسابهای دریافتنی
        /// 011430
        /// </summary>
        public static readonly AccountIndicator Account90 = new(283, "سایر حسابهای دریافتنی");
        /// <summary>
        /// سایر درآمدها
        /// 044710
        /// </summary>
        public static readonly AccountIndicator Account91 = new(284, "سایر درآمدها");
        /// <summary>
        /// سرمایه گذاری در سپرده بانکی
        /// 011510
        /// </summary>
        public static readonly AccountIndicator Account103 = new(287, "سرمایه گذاری در سپرده بانکی");
        /// <summary>
        /// سرمایه گذاری در گواهی سپرده بانکی
        /// 011520
        /// </summary>
        public static readonly AccountIndicator Account104 = new(288, "سرمایه گذاری در گواهی سپرده بانکی");
        /// <summary>
        /// سود اوراق مشارکت
        /// 044410
        /// </summary>
        public static readonly AccountIndicator BondsInterest = new(292, "سود اوراق مشارکت");
        /// <summary>
        /// سود ترجیحی دریافتنی اوراق بهادار با درآمد ثابت
        /// 011235
        /// </summary>
        public static readonly AccountIndicator Account109 = new(293, "سود ترجیحی دریافتنی اوراق بهادار با درآمد ثابت");
        /// <summary>
        /// سود دریافتنی از صندوق های سرمایه گذاری
        /// 011280
        /// </summary>
        public static readonly AccountIndicator Account110 = new(294, "سود دریافتنی از صندوق های سرمایه گذاری");
        /// <summary>
        /// سود دریافتنی سپرده بانکی
        /// 011210
        /// </summary>
        public static readonly AccountIndicator AccruedBankDepositInterest = new(296, "سود دریافتنی سپرده بانکی");
        /// <summary>
        /// سود دریافتنی گواهی سپرده بانکی
        /// 011220
        /// </summary>
        public static readonly AccountIndicator Account113 = new(297, "سود دریافتنی گواهی سپرده بانکی");
        /// <summary>
        /// سود دریافتنی وجه تضمین نزد سپرده گذاری
        /// 011285
        /// </summary>
        public static readonly AccountIndicator Account114 = new(298, "سود دریافتنی وجه تضمین نزد سپرده گذاری");
        /// <summary>
        /// سود و زیان ناشی از تسویه اختیار
        /// 044190
        /// </summary>
        public static readonly AccountIndicator Account124 = new(308, "سود و زیان ناشی از تسویه اختیار");
        /// <summary>
        /// سود(زیان) فروش اوراق مشارکت
        /// 044130
        /// </summary>
        public static readonly AccountIndicator BondsSoldRevenue = new(312, "سود(زیان) فروش اوراق مشارکت");
        /// <summary>
        /// سود(زیان) فروش حق تقدم سهام
        /// 044120
        /// </summary>
        public static readonly AccountIndicator RightSoldRevenue = new(314, "سود(زیان) فروش حق تقدم سهام");
        /// <summary>
        /// سود(زیان) ناشی از فروش سلف
        /// 044180
        /// </summary>
        public static readonly AccountIndicator SalafSoldRevenue = new(315, "سود(زیان) ناشی از فروش سلف");
        /// <summary>
        /// سود(زیان) ناشی از فروش صندوق های سرمایه گذاری
        /// 044170
        /// </summary>
        public static readonly AccountIndicator FundsSoldRevenue = new(318, "سود(زیان) ناشی از فروش صندوق های سرمایه گذاری");
        /// <summary>
        /// حساب ارزشیابی صندوق های سرمایه گذاری
        /// 011920
        /// </summary>
        public static readonly AccountIndicator FundsValuation = new(322, "حساب ارزشیابی صندوق های سرمایه گذاری");
        /// <summary>
        /// سرمایه گذاری در صندوق های سرمایه گذاری
        /// 011910
        /// </summary>
        public static readonly AccountIndicator FundsInvesting = new(323, "سرمایه گذاری در صندوق های سرمایه گذاری");
        /// <summary>
        /// سرمایه گذاری در اوراق سلف
        /// 011934
        /// </summary>
        public static readonly AccountIndicator SalafInvesting = new(324, "سرمایه گذاری در اوراق سلف");
        /// <summary>
        /// مالیات پرداختنی به مدیر (مدیر ثبت) بابت صدور و ابطال
        /// 022351
        /// </summary>
        public static readonly AccountIndicator Account141 = new(325, "مالیات پرداختنی به مدیر (مدیر ثبت) بابت صدور و ابطال");
        /// <summary>
        /// ذخیره مالیات ارکان
        /// 022460
        /// </summary>
        public static readonly AccountIndicator Account142 = new(326, "ذخیره مالیات ارکان");
        /// <summary>
        /// مالیات ذخیره هزینه نرم افزار
        /// 011861
        /// </summary>
        public static readonly AccountIndicator Account143 = new(327, "مالیات ذخیره هزینه نرم افزار");
        /// <summary>
        /// مخارج عضویت در کانون ها
        /// 011815
        /// </summary>
        public static readonly AccountIndicator Account145 = new(329, "مخارج اشتراک خدمات سپرده گذاری و نگهداری اوراق");
        /// <summary>
        /// مخارج انتقالی تاسیس
        /// 011810
        /// </summary>
        public static readonly AccountIndicator Account146 = new(330, "مخارج انتقالی تاسیس");
        /// <summary>
        /// مخارج عضویت در کانونها
        /// 011820
        /// </summary>
        public static readonly AccountIndicator Account147 = new(331, "مخارج عضویت در کانون‌ها");
        /// <summary>
        /// مطالبات بابت فروش حق تقدم
        /// 011290
        /// </summary>
        public static readonly AccountIndicator Account148 = new(332, "مطالبات بابت فروش حق تقدم");
        /// <summary>
        /// هزینه آبونمان نرم افزار
        /// 055397
        /// </summary>
        public static readonly AccountIndicator Account149 = new(333, "هزینه آبونمان نرم افزار");
        /// <summary>
        /// هزینه برگزاری مجامع
        /// 055210
        /// </summary>
        public static readonly AccountIndicator Account150 = new(334, "هزینه برگزاری مجامع");
        /// <summary>
        /// هزینه برگشت سود گواهی بانکی
        /// 055370
        /// </summary>
        public static readonly AccountIndicator Account151 = new(335, "هزینه برگشت سود گواهی بانکی");
        /// <summary>
        /// هزینه تاسیس
        /// 055110
        /// </summary>
        public static readonly AccountIndicator Account152 = new(336, "هزینه تاسیس");
        /// <summary>
        /// هزینه تسهیلات بانکی
        /// 055510
        /// </summary>
        public static readonly AccountIndicator Account153 = new(337, "هزینه تسهیلات بانکی");
        /// <summary>
        /// هزینه تصفیه
        /// 055310
        /// </summary>
        public static readonly AccountIndicator Account154 = new(338, "هزینه تصفیه");
        /// <summary>
        /// هزینه تنزیل سود دریافتنی سپرده بانکی
        /// 055350
        /// </summary>
        public static readonly AccountIndicator Account155 = new(339, "هزینه تنزیل سود دریافتنی سپرده بانکی");
        /// <summary>
        /// هزینه حق الزحمه حسابرس
        /// 055040
        /// </summary>
        public static readonly AccountIndicator Account156 = new(340, "هزینه حق الزحمه حسابرس");
        /// <summary>
        /// هزینه دعاوی حقوقی
        /// 055330
        /// </summary>
        public static readonly AccountIndicator Account157 = new(341, "هزینه دعاوی حقوقی");
        /// <summary>
        /// هزینه رتبه بندی صندق های سرمایه گذاری
        /// 055390
        /// </summary>
        public static readonly AccountIndicator Account158 = new(342, "هزینه رتبه بندی صندق های سرمایه گذاری");
        /// <summary>
        /// هزینه کارمزد ابطال واحد صندوق غیر بورسی
        /// 055396
        /// </summary>
        public static readonly AccountIndicator Account159 = new(343, "هزینه کارمزد ابطال واحد صندوق غیر بورسی");
        /// <summary>
        /// هزینه کارمزد بانکی
        /// 055380
        /// </summary>
        public static readonly AccountIndicator Account160 = new(344, "هزینه کارمزد بانکی");
        /// <summary>
        /// هزینه کارمزد ضامن
        /// 055030
        /// </summary>
        public static readonly AccountIndicator Account161 = new(345, "هزینه کارمزد ضامن");
        /// <summary>
        /// هزینه کارمزد متولی
        /// 055020
        /// </summary>
        public static readonly AccountIndicator Account164 = new(348, "هزینه کارمزد متولی");
        /// <summary>
        /// هزینه کارمزد مدیر
        /// 055010
        /// </summary>
        public static readonly AccountIndicator ManagementFeeExpense = new(349, "هزینه کارمزد مدیر");
        /// <summary>
        /// هزینه نگهداری اوراق بهادار بی نام
        /// 055320
        /// </summary>
        public static readonly AccountIndicator Account173 = new(356, "هزینه نگهداری اوراق بهادار بی نام");
        /// <summary>
        /// هزینه نرم افزار
        /// 055190
        /// </summary>
        public static readonly AccountIndicator SoftwareVariableCost = new(357, "هزینه نرم افزار");
        /// <summary>
        /// واحدهای سرمایه گذاری ممتاز
        /// 033130
        /// </summary>
        public static readonly AccountIndicator Account175 = new(358, "واحدهای سرمایه گذاری ممتاز");
        /// <summary>
        /// وجه تضمین نزد سپرده گذاری
        /// 011431
        /// </summary>
        public static readonly AccountIndicator Account176 = new(359, "وجه تضمین نزد سپرده گذاری");
        /// <summary>
        /// وجوه واریزی نامشخص
        /// 022530
        /// </summary>
        public static readonly AccountIndicator Account177 = new(360, "وجوه واریزی نامشخص");
        /// <summary>
        /// نقد
        /// 011120
        /// </summary>
        public static readonly AccountIndicator Cash = new(361, "نقد");
        /// <summary>
        /// مالیات آبونمان نرم افزار صندوق
        /// 011871
        /// </summary>
        public static readonly AccountIndicator Account178 = new(362, "مالیات آبونمان نرم افزار صندوق");
        /// <summary>
        /// عوارض آبونمان نرم افزار صندوق
        /// 011872
        /// </summary>
        public static readonly AccountIndicator Account179 = new(363, "عوارض آبونمان نرم افزار صندوق");
        /// <summary>
        /// ذخیره مالیات هزینه نرم افزار
        /// 022541
        /// </summary>
        public static readonly AccountIndicator Account180 = new(364, "ذخیره مالیات هزینه نرم افزار");
        /// <summary>
        /// ذخیره عوارض هزینه نرم افزار
        /// 022542
        /// </summary>
        public static readonly AccountIndicator Account181 = new(365, "ذخیره عوارض هزینه نرم افزار");
        /// <summary>
        /// مالیات هزینه آبونمان نرم افزار
        /// 055398
        /// </summary>
        public static readonly AccountIndicator Account182 = new(366, "مالیات هزینه آبونمان نرم افزار");
        /// <summary>
        /// عوارض هزینه آبونمان نرم افزار
        /// 055399
        /// </summary>
        public static readonly AccountIndicator Account183 = new(367, "عوارض هزینه آبونمان نرم افزار");
        /// <summary>
        /// هزینه مالیات نرم افزار
        /// 055191
        /// </summary>
        public static readonly AccountIndicator Account184 = new(368, "هزینه مالیات نرم افزار");
        /// <summary>
        /// هزینه عوارض  نرم افزار
        /// 055192
        /// </summary>
        public static readonly AccountIndicator Account185 = new(369, "هزینه عوارض  نرم افزار");
        /// <summary>
        /// ذخیره عوارض ارکان
        /// 022465
        /// </summary>
        public static readonly AccountIndicator Account186 = new(370, "ذخیره عوارض ارکان");
        /// <summary>
        /// هزینه کارمزد مدیر مبتنی بر عملکرد
        /// 055011
        /// </summary>
        public static readonly AccountIndicator Account187 = new(371, "هزینه کارمزد مدیر مبتنی بر عملکرد");
        /// <summary>
        /// ذخیره کارمزد مدیر مبتنی بر عملکرد
        /// 022411
        /// </summary>
        public static readonly AccountIndicator Account188 = new(372, "ذخیره کارمزد مدیر مبتنی بر عملکرد");
        /// <summary>
        /// هزینه مالیات ارکان
        /// 055060
        /// </summary>
        public static readonly AccountIndicator Account189 = new(373, "هزینه مالیات ارکان");
        /// <summary>
        /// هزینه عوارض ارکان
        /// 055065
        /// </summary>
        public static readonly AccountIndicator Account190 = new(374, "هزینه عوارض ارکان");
        /// <summary>
        /// حساب ارزشیابی اوراق سلف
        /// 011935
        /// </summary>
        public static readonly AccountIndicator SalafValuation = new(376, "حساب ارزشیابی اوراق سلف");
        /// <summary>
        /// سرمایه گذاری در گواهی سپرده کالایی
        /// 011715
        /// </summary>
        public static readonly AccountIndicator Account192 = new(377, "سرمایه گذاری در گواهی سپرده کالایی");
        /// <summary>
        /// هزینه کارمزد بازارگردان
        /// 055050
        /// </summary>
        public static readonly AccountIndicator Account204 = new(378, "هزینه کارمزد بازارگردان");
        /// <summary>
        /// هزینه کارمزد ضامن سودآوری
        /// 055070
        /// </summary>
        public static readonly AccountIndicator Account205 = new(379, "هزینه کارمزد ضامن سودآوری");
        /// <summary>
        /// هزینه کارمزد ضامن نقدشوندگی
        /// 055080
        /// </summary>
        public static readonly AccountIndicator Account206 = new(380, "هزینه کارمزد ضامن نقدشوندگی");
        /// <summary>
        /// هزینه کارمزد مدیر ثبت
        /// 055090
        /// </summary>
        public static readonly AccountIndicator Account207 = new(381, "هزینه کارمزد مدیر ثبت");
        /// <summary>
        /// ذخیره کارمزد بازارگردان
        /// 022450
        /// </summary>
        public static readonly AccountIndicator Account208 = new(382, "ذخیره کارمزد بازارگردان");
        /// <summary>
        /// ذخیره کارمزد ضامن سودآوری
        /// 022470
        /// </summary>
        public static readonly AccountIndicator Account209 = new(383, "ذخیره کارمزد ضامن سودآوری");
        /// <summary>
        /// ذخیره کارمزد ضامن نقدشوندگی
        /// 022480
        /// </summary>
        public static readonly AccountIndicator Account210 = new(384, "ذخیره کارمزد ضامن نقدشوندگی");
        /// <summary>
        /// ذخیره کارمزد مدیر ثبت
        /// 022490
        /// </summary>
        public static readonly AccountIndicator Account211 = new(385, "ذخیره کارمزد مدیر ثبت");
        /// <summary>
        /// هزینه کارمزد مدیر سرمایه گذاری
        /// 055095
        /// </summary>
        public static readonly AccountIndicator Account212 = new(386, "هزینه کارمزد مدیر سرمایه گذاری");
        /// <summary>
        /// ذخیره کارمزد مدیر سرمایه گذاری
        /// 022495
        /// </summary>
        public static readonly AccountIndicator Account213 = new(387, "ذخیره کارمزد مدیر سرمایه گذاری");
        /// <summary>
        /// هزینه کارمزد متولی مبتنی بر عملکرد
        /// 055021
        /// </summary>
        public static readonly AccountIndicator Account214 = new(388, "هزینه کارمزد متولی مبتنی بر عملکرد");
        /// <summary>
        /// ذخیره کارمزد متولی مبتنی بر عملکرد
        /// 022421
        /// </summary>
        public static readonly AccountIndicator Account215 = new(389, "ذخیره کارمزد متولی مبتنی بر عملکرد");
        /// <summary>
        /// هزینه کارمزد مدیر ثبت مبتنی بر عملکرد
        /// 055012
        /// </summary>
        public static readonly AccountIndicator Account216 = new(390, "هزینه کارمزد مدیر ثبت مبتنی بر عملکرد");
        /// <summary>
        /// ذخیره کارمزد مدیر ثبت مبتنی بر عملکرد
        /// 022412
        /// </summary>
        public static readonly AccountIndicator Account217 = new(391, "ذخیره کارمزد مدیر ثبت مبتنی بر عملکرد");
        /// <summary>
        /// هزینه کارمزد مدیر سرمایه گذاری مبتنی بر عملکرد
        /// 055013
        /// </summary>
        public static readonly AccountIndicator Account218 = new(392, "هزینه کارمزد مدیر سرمایه گذاری مبتنی بر عملکرد");
        /// <summary>
        /// ذخیره کارمزد مدیر سرمایه گذاری مبتنی بر عملکرد
        /// 022413
        /// </summary>
        public static readonly AccountIndicator Account219 = new(393, "ذخیره کارمزد مدیر سرمایه گذاری مبتنی بر عملکرد");
        /// <summary>
        /// هزینه کارمزد ضامن نقدشوندگی مبتنی بر عملکرد
        /// 055031
        /// </summary>
        public static readonly AccountIndicator Account220 = new(394, "هزینه کارمزد ضامن نقدشوندگی مبتنی بر عملکرد");
        /// <summary>
        /// ذخیره کارمزد ضامن نقدشوندگی مبتنی بر عملکرد
        /// 022431
        /// </summary>
        public static readonly AccountIndicator Account221 = new(395, "ذخیره کارمزد ضامن نقدشوندگی مبتنی بر عملکرد");
        /// <summary>
        /// هزینه کارمزد ضامن سودآوری مبتنی بر عملکرد
        /// 055032
        /// </summary>
        public static readonly AccountIndicator Account222 = new(396, "هزینه کارمزد ضامن سودآوری مبتنی بر عملکرد");
        /// <summary>
        /// ذخیره کارمزد ضامن سودآوری مبتنی بر عملکرد
        /// 022432
        /// </summary>
        public static readonly AccountIndicator Account223 = new(397, "ذخیره کارمزد ضامن سودآوری مبتنی بر عملکرد");
        /// <summary>
        /// هزینه کارمزد بازارگردان مبتنی بر عملکرد
        /// 055051
        /// </summary>
        public static readonly AccountIndicator Account224 = new(398, "هزینه کارمزد بازارگردان مبتنی بر عملکرد");
        /// <summary>
        /// ذخیره کارمزد بازارگردان مبتنی بر عملکرد
        /// 022451
        /// </summary>
        public static readonly AccountIndicator Account225 = new(399, "ذخیره کارمزد بازارگردان مبتنی بر عملکرد");
        /// <summary>
        /// هزینه مالیات هزینه های ثابت صندوق
        /// 011890
        /// </summary>
        public static readonly AccountIndicator Account226 = new(400, "هزینه مالیات هزینه های ثابت صندوق");
        /// <summary>
        /// هزینه عوارض هزینه های ثابت صندوق
        /// 011895
        /// </summary>
        public static readonly AccountIndicator Account227 = new(401, "هزینه عوارض هزینه های ثابت صندوق");
        /// <summary>
        /// ذخیره مالیات هزینه های ثابت صندوق
        /// 022590
        /// </summary>
        public static readonly AccountIndicator Account228 = new(402, "ذخیره مالیات هزینه های ثابت صندوق");
        /// <summary>
        /// ذخیره عوارض هزینه های ثابت صندوق
        /// 022595
        /// </summary>  
        public static readonly AccountIndicator Account229 = new(403, "ذخیره عوارض هزینه های ثابت صندوق");
        /// <summary>
        /// مخارج انتقالی برگزاری مجامع
        /// 011830
        /// </summary>
        public static readonly AccountIndicator Account230 = new(404, "مخارج انتقالی برگزاری مجامع");
        /// <summary>
        /// حساب اررشیابی در گواهی سپرده بانکی
        /// 011525
        /// </summary>
        public static readonly AccountIndicator BankDepositeValuation = new(405, "حساب اررشیابی در گواهی سپرده بانکی");
        /// <summary>
        /// پرداختنی به بازارگردان
        /// 022360
        /// </summary>
        public static readonly AccountIndicator PayableToMarketer = new(406, "پرداختنی به بازارگردان");
        /// <summary>
        /// جاری کارگزار از تعدیل کارمزد
        /// 011320
        /// </summary>
        public static readonly AccountIndicator Account231 = new(407, "جاری کارگزار از تعدیل کارمزد");
        /// <summary>
        /// جاری کارگزار بابت صدور و ابطال بازارگردان
        /// 011380
        /// </summary>
        public static readonly AccountIndicator Account232 = new(408, "جاری کارگزار بابت صدور و ابطال بازارگردان");
        /// <summary>
        /// مخارج افزایش سقف واحدهای سرمایه گذاری صندوق
        /// 011840
        /// </summary>
        public static readonly AccountIndicator CostOfIncreasingMaxUnits = new(409, "مخارج افزایش سقف واحدهای سرمایه گذاری صندوق");
        /// <summary>
        /// مخارج رتبه بندی
        /// 011850
        /// </summary>
        public static readonly AccountIndicator RatingCosts = new(410, "مخارج رتبه بندی");
        /// <summary>
        /// حساب ارزشیابی گواهی سپرده کالایی
        /// 011725
        /// </summary>
        public static readonly AccountIndicator ImeValuation = new(411, "حساب ارزشیابی گواهی سپرده کالایی");
        /// <summary>
        /// هزینه انبارداری
        /// 055315
        /// </summary>
        public static readonly AccountIndicator WarehousesCosts = new(412, "هزینه انبارداری");
        /// <summary>
        /// ذخیره انبارداری
        /// 022515
        /// </summary>
        public static readonly AccountIndicator WarehousesCostsReserve = new(413, "ذخیره انبارداری");
        /// <summary>
        /// سود و زیان ناشی از فروش گواهی سپرده کالایی
        /// 044118
        /// </summary>
        public static readonly AccountIndicator DepositCertificateSoldRevenue = new(414, "سود و زیان ناشی از فروش گواهی سپرده کالایی");
        /// <summary>
        /// درآمد (هزینه) تغییر ارزش سرمایه گذاری در گواهی سپرده کالایی
        /// 044515
        /// </summary>
        public static readonly AccountIndicator DepositCertificateValueChangesIncome = new(415, "درآمد (هزینه) تغییر ارزش سرمایه گذاری در گواهی سپرده کالایی");
        /// <summary>
        /// هزینه کارمزد فروش حق تقدم استفاده نشده
        /// 055420
        /// </summary>
        public static readonly AccountIndicator UnusedRightSoldCommission = new(416, "هزینه کارمزد فروش حق تقدم استفاده نشده");
        /// <summary>
        /// پیش دریافت سود سپرده بانکی
        /// 022010
        /// </summary>
        public static readonly AccountIndicator AdvancedBankDepositProfits = new (417, "پیش دریافت سود سپرده بانکی");
        /// <summary>
        /// پیش دریافت سود اوراق بهادار با درآمد ثابت
        /// 022020
        /// </summary>
        public static readonly AccountIndicator AdvancedFixedIncomeProfit = new(418, "پیش دریافت سود اوراق بهادار با درآمد ثابت");
        /// <summary>
        /// سایر هزینه ها
        /// 055395
        /// </summary>
        public static readonly AccountIndicator OtherCosts = new(420, "سایر هزینه ها");
        /// <summary>
        /// سود تقسیمی صندوق های سرمایه گذاری
        /// 044910
        /// </summary>
        public static readonly AccountIndicator FundDividend = new (421, "سود تقسیمی صندوق های سرمایه گذاری");
        /// <summary>
        /// حساب ارزشيابي در صندوق های در سهام قابل معامله در بورس
        /// 011911
        /// </summary>
        public static readonly AccountIndicator FundsValuationStockTSE = new(422, "حساب ارزشيابي در صندوق های در سهام قابل معامله در بورس");
        /// <summary>
        /// صندوق های در سهام قابل معامله در فرابورس
        /// 011912
        /// </summary>
        public static readonly AccountIndicator FundsInvestingStockIFB = new(423, "صندوق های در سهام قابل معامله در فرابورس");
        /// <summary>
        /// حساب ارزشيابي در صندوق های در سهام قابل معامله در فرابورس
        /// 011913
        /// </summary>
        public static readonly AccountIndicator FundsValuationStockIFB = new(424, "حساب ارزشيابي در صندوق های در سهام قابل معامله در فرابورس");
        /// <summary>
        /// صندوق های در سهام  غیر قابل معامله
        /// 011914
        /// </summary>
        public static readonly AccountIndicator FundsInvestingStockUnlisted = new(425, "صندوق های در سهام  غیر قابل معامله");
        /// <summary>
        /// حساب ارزشيابي در صندوق های در سهام غیر قابل معامله
        /// 011915
        /// </summary>
        public static readonly AccountIndicator FundsValuationStockUnlisted = new(426, "حساب ارزشيابي در صندوق های در سهام غیر قابل معامله");
        /// <summary>
        /// صندوق های مختلط قابل معامله در بورس
        /// 011922
        /// </summary>
        public static readonly AccountIndicator FundsInvestingMixedTSE = new(427, "صندوق های مختلط قابل معامله در بورس");
        /// <summary>
        /// حساب ارزشيابي در صندوق های مختلط قابل معامله در بورس
        /// 011923
        /// </summary>
        public static readonly AccountIndicator FundsValuationMixedTSE = new(428, "حساب ارزشيابي در صندوق های مختلط قابل معامله در بورس");
        /// <summary>
        /// صندوق های مختلط قابل معامله در فرابورس
        /// 011924
        /// </summary>
        public static readonly AccountIndicator FundsInvestingMixedIFB = new(429, "صندوق های مختلط قابل معامله در فرابورس");
        /// <summary>
        /// حساب ارزشيابي در صندوق های مختلط قابل معامله در فرابورس
        /// 011925
        /// </summary>
        public static readonly AccountIndicator FundsValuationMixedIFB = new(430, "حساب ارزشيابي در صندوق های مختلط قابل معامله در فرابورس");
        /// <summary>
        /// صندوق های مختلط  غیر قابل معامله
        /// 011926
        /// </summary>
        public static readonly AccountIndicator FundsInvestingMixedUnlisted = new(431, "صندوق های مختلط  غیر قابل معامله");
        /// <summary>
        /// حساب ارزشيابي در صندوق های مختلط  غیر قابل معامله
        /// 011927
        /// </summary>
        public static readonly AccountIndicator FundsValuationMixedUnlisted = new(432, "حساب ارزشيابي در صندوق های مختلط  غیر قابل معامله");
        /// <summary>
        /// صندوق های زمين و ساختمان قابل معامله در بورس
        /// 011928
        /// </summary>
        public static readonly AccountIndicator FundsInvestingRealEstateTSE = new(433, "صندوق های زمين و ساختمان قابل معامله در بورس");
        /// <summary>
        /// حساب ارزشيابي در صندوق های زمين و ساختمان قابل معامله در بورس
        /// 011929
        /// </summary>
        public static readonly AccountIndicator FundsValuationRealEstateTSE = new(434, "حساب ارزشيابي در صندوق های زمين و ساختمان قابل معامله در بورس");
        /// <summary>
        /// صندوق های زمين و ساختمان قابل معامله در فرابورس
        /// 011930
        /// </summary>
        public static readonly AccountIndicator FundsInvestingRealEstateIFB = new(435, "صندوق های زمين و ساختمان قابل معامله در فرابورس");
        /// <summary>
        /// حساب ارزشيابي در صندوق های زمين و ساختمان قابل معامله در فرابورس
        /// 011931
        /// </summary>
        public static readonly AccountIndicator FundsValuationRealEstateIFB = new(436, "حساب ارزشيابي در صندوق های زمين و ساختمان قابل معامله در فرابورس");
        /// <summary>
        /// صندوق های پروژه قابل معامله در بورس
        /// 011936
        /// </summary>
        public static readonly AccountIndicator FundsInvestingProjectTSE = new(437, "صندوق های پروژه قابل معامله در بورس");
        /// <summary>
        /// صندوق های پروژه غیر قابل معامله
        /// 011937
        /// </summary>
        public static readonly AccountIndicator FundsValuationProjectTSE = new(438, "صندوق های پروژه غیر قابل معامله");
        /// <summary>
        /// صندوق های پروژه قابل معامله در فرابورس
        /// 011938
        /// </summary>
        public static readonly AccountIndicator FundsInvestingProjectIFB = new(439, "صندوق های پروژه قابل معامله در فرابورس");
        /// <summary>
        /// حساب ارزشيابي در صندوق های پروژه قابل معامله در فرابورس
        /// 011939
        /// </summary>
        public static readonly AccountIndicator FundsValuationProjectIFB = new(440, "حساب ارزشيابي در صندوق های پروژه قابل معامله در فرابورس");
        /// <summary>
        /// صندوق های درآمد ثابت قابل معامله در بورس
        /// 011916
        /// </summary>
        public static readonly AccountIndicator FundsInvestingFixedIncomeTSE = new(441, "صندوق های درآمد ثابت قابل معامله در بورس");
        /// <summary>
        /// حساب ارزشيابي در صندوق های پروژه قابل معامله در فرابورس
        /// 011917
        /// </summary>
        public static readonly AccountIndicator FundsValuationFixedIncomeTSE = new(442, "حساب ارزشیابی صندوق های درآمد ثابت قابل معامله در بورس");
        /// <summary>
        /// صندوق های درآمد ثابت قابل معامله در فرابورس
        /// 011918
        /// </summary>
        public static readonly AccountIndicator FundsInvestingFixedIncomeIFB = new(443, "صندوق های درآمد ثابت قابل معامله در فرابورس");
        /// <summary>
        /// حساب ارزشیابی صندوق های درآمد ثابت قابل معامله در فرابورس
        /// 011919
        /// </summary>
        public static readonly AccountIndicator FundsValuationFixedIncomeIFB = new(444, "حساب ارزشیابی صندوق های درآمد ثابت قابل معامله در فرابورس");
        /// <summary>
        /// صندوق های درآمد ثابت غیر قابل معامله
        /// 011932
        /// </summary>
        public static readonly AccountIndicator FundsInvestingFixedIncomeUnlisted = new(445, "صندوق های درآمد ثابت غیر قابل معامله");
        /// <summary>
        /// حساب ارزشیابی صندوق های درآمد ثابت غیر قابل معامله
        /// 011933
        /// </summary>
        public static readonly AccountIndicator FundsValuationFixedIncomeUnlisted = new(446, "حساب ارزشیابی صندوق های درآمد ثابت غیر قابل معامله");
        /// <summary>
        /// سود فروش سرمایه گذاری در صندوق های در سهام غير قابل معامله
        /// 044174
        /// </summary>
        public static readonly AccountIndicator StockFundProfitSaleUnlisted = new(447, "سود فروش سرمایه گذاری در صندوق های در سهام غير قابل معامله");
        /// <summary>
        /// زیان فروش سرمایه گذاری در صندوق های در سهام غير قابل معامله
        /// 044175
        /// </summary>
        public static readonly AccountIndicator StockFundLossSaleUnlisted = new(448, "زیان فروش سرمایه گذاری در صندوق های در سهام غير قابل معامله");
        /// <summary>
        /// سود فروش سرمایه گذاری در صندوق های درآمد ثابت  غير قابل معامله
        /// 044176
        /// </summary>
        public static readonly AccountIndicator FixedIncomeFundProfitFromSaleUnlisted = new(449, "سود فروش سرمایه گذاری در صندوق های درآمد ثابت  غير قابل معامله");
        /// <summary>
        /// زیان فروش سرمایه گذاری در صندوق های درآمد ثابت  غير قابل معامله
        /// 044177
        /// </summary>
        public static readonly AccountIndicator FixedIncomeFundLossFromSaleUnlisted = new(450, "زیان فروش سرمایه گذاری در صندوق های درآمد ثابت  غير قابل معامله");
        /// <summary>
        /// سود فروش سرمایه گذاری در صندوق های مختلط  غير قابل معامله
        /// 044186
        /// </summary>
        public static readonly AccountIndicator MixedFundProfitFromSaleUnlisted = new(451, "سود فروش سرمایه گذاری در صندوق های مختلط  غير قابل معامله");
        /// <summary>
        /// زیان فروش سرمایه گذاری در صندوق های مختلط  غير قابل معامله
        /// 044187
        /// </summary>
        public static readonly AccountIndicator MixedFundLossFromSaleUnlisted = new(452, "زیان فروش سرمایه گذاری در صندوق های مختلط  غير قابل معامله");
        /// <summary>
        /// درآمد (هزينه) تغيير ارزش سرمایه گذاری در صندوق های در سهام غیرقابل معامله
        /// 044562
        /// </summary>
        public static readonly AccountIndicator StockFundChangeIncome = new(453, "درآمد (هزينه) تغيير ارزش سرمایه گذاری در صندوق های در سهام غیرقابل معامله");
        /// <summary>
        /// درآمد (هزينه) تغيير ارزش سرمایه گذاری در صندوق های مختلط غیرقابل معامله
        /// 044568
        /// </summary>
        public static readonly AccountIndicator MixedFundChangeIncome = new(454, "درآمد (هزينه) تغيير ارزش سرمایه گذاری در صندوق های مختلط غیرقابل معامله");
        /// <summary>
        /// درآمد (هزينه) تغيير ارزش سرمایه گذاری در صندوق های پروژه غیرقابل معامله
        /// 044565
        /// </summary>
        public static readonly AccountIndicator ProjectFundChangeIncome = new(455, "درآمد (هزينه) تغيير ارزش سرمایه گذاری در صندوق های پروژه غیرقابل معامله");
        /// <summary>
        /// درآمد (هزينه) تغيير ارزش سرمایه گذاری در صندوق های درآمد ثابت غیرقابل معامله
        /// 044564
        /// </summary>
        public static readonly AccountIndicator FixedIncomeFundChangeIncome = new(456, "درآمد (هزينه) تغيير ارزش سرمایه گذاری در صندوق های درآمد ثابت غیرقابل معامله");
        /// <summary>
        /// سود فروش سرمایه گذاری در صندوق های پروژه غير قابل معامله
        /// 044116
        /// </summary>
        public static readonly AccountIndicator ProjectFundProfitFromSaleUnlisted = new(457, "سود فروش سرمایه گذاری در صندوق های پروژه غير قابل معامله");
        /// <summary>
        /// زیان فروش سرمایه گذاری در صندوق های پروژه غير قابل معامله
        /// 044117
        /// </summary>
        public static readonly AccountIndicator ProjectFundLossFromSaleUnlisted = new(458, "زیان فروش سرمایه گذاری در صندوق های پروژه غير قابل معامله");
        /// <summary>
        /// حساب تنزیل سود دریافتنی اوراق
        /// 055365
        /// </summary>
        public static readonly AccountIndicator BondsDiscountIncomeReceivable = new(459, "حساب تنزیل سود دریافتنی اوراق");
        /// <summary>
        /// درآمد (هزينه) تغيير ارزش سرمایه گذاری در صندوق های غیرقابل معامله
        /// 044561
        /// </summary>
        public static readonly AccountIndicator OTCFundsValueChangesIncome = new(460, "درآمد (هزينه) تغيير ارزش سرمایه گذاری در صندوق های غیرقابل معامله");
        /// <summary>
        /// بدهی به سرمایه گذاران بابت پرداخت ذخیره ناشی از تغییرات ارزش سهام
        /// 022270
        /// </summary>
        public static readonly AccountIndicator InvestorLiabilityForValueChangeReserve = new(461, "بدهی به سرمایه گذاران بابت پرداخت ذخیره ناشی از تغییرات ارزش سهام");
        /// <summary>
        /// سرمایه گذاری در اوراق تبعی تامین مالی
        /// 011630
        /// </summary>
        //public static readonly AccountIndicator FinancedEmbeddedsInvesting = new(462, "سرمایه گذاری در اوراق تبعی تامین مالی");
        /// <summary>
        /// حساب ارزشیابی در اوراق تبعی تامین مالی
        /// 011635
        /// </summary>
        //public static readonly AccountIndicator FinancedEmbeddedsValuation = new(463, "حساب ارزشیابی در اوراق تبعی تامین مالی");
        /// <summary>
        /// درآمد (هزینه) تغییر ارزش سرمایه گذاری در اوراق تبعی حمایتی
        /// 044580
        /// </summary>
        //public static readonly AccountIndicator SupportiveEmbeddedsChangeIncome = new(464, "درآمد (هزینه) تغییر ارزش سرمایه گذاری در اوراق تبعی حمایتی");
        /// <summary>
        /// درآمد (هزینه) تغییر ارزش سرمایه گذاری در اوراق تبعی تامین مالی
        /// 044585
        /// </summary>
        //public static readonly AccountIndicator FinancedEmbeddedsChangeIncome = new(465, "درآمد (هزینه) تغییر ارزش سرمایه گذاری در اوراق تبعی تامین مالی");
        /// <summary>
        /// سرمایه گذاری در اوراق تبعی حمایتی
        /// 011760
        /// </summary>
        //public static readonly AccountIndicator SupportiveEmbeddedsInvesting = new(466, "سرمایه گذاری در اوراق تبعی حمایتی");
        /// <summary>
        /// حساب ارزشیابی در اوراق تبعی حمایتی
        /// 011765
        /// </summary>
        //public static readonly AccountIndicator SupportiveEmbeddedsValuation = new(467, "حساب ارزشیابی در اوراق تبعی حمایتی");
        /// <summary>
        /// سود و زیان ناشی از تسویه اوراق تبعی حمایتی
        /// 044195
        /// </summary>
        public static readonly AccountIndicator SupportiveEmbeddedsSettlementRevenue = new(468, "سود و زیان ناشی از تسویه اوراق تبعی حمایتی");
        /// <summary>
        /// سود و زیان ناشی از تسویه اوراق تبعی تامین مالی
        /// 044196
        /// </summary>
        public static readonly AccountIndicator FinancedEmbeddedsSettlementRevenue = new(469, "سود و زیان ناشی از تسویه اوراق تبعی تامین مالی");
        /// <summary>
        /// سود(زیان) ناشی از ابطال اوراق تبعی حمایتی
        /// 044115
        /// </summary>
        public static readonly AccountIndicator SupportiveEmbeddedsCancelationRevenue = new(470, "سود(زیان) ناشی از ابطال اوراق تبعی حمایتی");
        /// <summary>
        /// سود(زیان) ناشی از ابطال اوراق تبعی تامین مالی
        /// 044135
        /// </summary>
        public static readonly AccountIndicator FinancedEmbeddedsCancelationRevenue = new(471, "سود(زیان) ناشی از ابطال اوراق تبعی تامین مالی");
        /// <summary>
        /// پرداختنی بابت اختیار تبعی
        /// 022130
        /// </summary>
        public static readonly AccountIndicator PayableToEmbeddedsTrade = new(472, "پرداختنی بابت اختیار تبعی");
        /// <summary>
        /// سود(زیان) فروش اوراق مشارکت غیر بورسی
        /// 044160
        /// </summary>
        public static readonly AccountIndicator NonListedBondSaleGainLoss = new(473, "سود(زیان) فروش اوراق مشارکت غیر بورسی");
        /// <summary>
        /// هزینه کارمزد فروش گواهی سپرده
        /// 055418
        /// </summary>
        public static readonly AccountIndicator CertificateSaleCommission = new(474, "هزینه کارمزد فروش گواهی سپرده");

        #endregion


        private AccountIndicator(int id, string name) : base(id, name)
        {

        }

        //public static explicit operator AccountIndicator(int id) => new(id);

        public static implicit operator int(AccountIndicator indicator) => indicator.Id;
    }
}
