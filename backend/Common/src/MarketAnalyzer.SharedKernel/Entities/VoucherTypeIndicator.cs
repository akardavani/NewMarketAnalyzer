using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.SharedKernel
{
    public class VoucherTypeIndicator : Enumeration
    {
        /// <summary>
        /// اعمال سود
        /// </summary>
        public static readonly VoucherTypeIndicator Dps = new(1, "اعمال سود");
        /// <summary>
        /// پذیره نویسی
        /// </summary>
        public static readonly VoucherTypeIndicator Underwriting = new(2, "پذیره نویسی");
        /// <summary>
        /// تایید درخواست صدور
        /// </summary>
        public static readonly VoucherTypeIndicator IssuanceRequestConfirmation = new(3, "تایید درخواست صدور");
        /// <summary>
        /// تایید درخواست ابطال
        /// </summary>
        public static readonly VoucherTypeIndicator CancellationRequestConfirmation = new(4, "تایید درخواست ابطال");
        /// <summary>
        /// خرید و فروش اوراق مشارکت
        /// </summary>
        public static readonly VoucherTypeIndicator BondsTrade = new(5, "خرید و فروش اوراق مشارکت");
        /// <summary>
        /// خرید و فروش سهم و حق تقدم صندوق
        /// </summary>
        public static readonly VoucherTypeIndicator StocksTrade = new(6, "خرید و فروش سهم و حق تقدم صندوق");
        /// <summary>
        /// محاسبات روزانه صندوق
        /// </summary>
        public static readonly VoucherTypeIndicator FundDailyCalculations = new(7, "محاسبات روزانه صندوق");
        /// <summary>
        /// عمومی
        /// </summary>
        public static readonly VoucherTypeIndicator General = new(8, "عمومی");
        /// <summary>
        /// دریافت سود سپرده
        /// </summary>
        public static readonly VoucherTypeIndicator ReceiveDepositInterest = new(9, "دریافت سود سپرده");
        /// <summary>
        /// دریافت سود سهام
        /// </summary>
        public static readonly VoucherTypeIndicator ReceiveDividends = new(10, "دریافت سود سهام");
        /// <summary>
        /// سود حاصل از خرید و فروش اوراق
        /// </summary>
        public static readonly VoucherTypeIndicator BondsTradeProfit = new(11, "سود حاصل از خرید و فروش اوراق");
        /// <summary>
        /// سند اصلاحیه
        /// </summary>
        public static readonly VoucherTypeIndicator AmendmentVoucher = new(12, "سند اصلاحیه");
        /// <summary>
        /// درخواست صدور
        /// </summary>
        public static readonly VoucherTypeIndicator IssuanceRequest = new(13, "درخواست صدور");
        /// <summary>
        /// رد درخواست صدور
        /// </summary>
        public static readonly VoucherTypeIndicator IssuanceRequestRejection = new(14, "رد درخواست صدور");
        /// <summary>
        /// پرداخت
        /// </summary>
        public static readonly VoucherTypeIndicator Payment = new(15, "پرداخت");
        /// <summary>
        /// انتقال وجه
        /// </summary>
        public static readonly VoucherTypeIndicator MoneyTransfer = new(16, "انتقال وجه");
        /// <summary>
        /// خرید فروش گواهی
        /// </summary>
        public static readonly VoucherTypeIndicator CertificateTrade = new(17, "خرید فروش گواهی");
        /// <summary>
        /// اعمال افزایش سرمایه
        /// </summary>
        public static readonly VoucherTypeIndicator CapitalChange = new(18, "اعمال افزایش سرمایه");
        /// <summary>
        /// پرداخت حق تقدم
        /// </summary>
        public static readonly VoucherTypeIndicator RightPayment = new(19, "پرداخت حق تقدم");
        /// <summary>
        /// مابه التفاوت واریز با صدور و ابطال
        /// </summary>
        public static readonly VoucherTypeIndicator Type20 = new(20, "مابه التفاوت واریز با صدور و ابطال");
        /// <summary>
        /// تنزیلات و تعدیلات
        /// </summary>
        public static readonly VoucherTypeIndicator DiscountsAdjustments = new(21, "تنزیلات و تعدیلات");
        /// <summary>
        /// تقسیم سود واحدها
        /// </summary>
        public static readonly VoucherTypeIndicator UnitsDividend = new(22, "تقسیم سود واحدها");
        /// <summary>
        /// افتتاحیه
        /// </summary>
        public static readonly VoucherTypeIndicator Opening = new(23, "افتتاحیه");
        /// <summary>
        /// اختتامیه
        /// </summary>
        public static readonly VoucherTypeIndicator Closing = new(24, "اختتامیه");
        /// <summary>
        /// سند بستن محاسبات صندوق بابت تعدیلات و تنزیلات
        /// </summary>
        public static readonly VoucherTypeIndicator Type25 = new(25, "سند بستن محاسبات صندوق بابت تعدیلات و تنزیلات");
        /// <summary>
        /// بستن حسابهای موقت
        /// </summary>
        public static readonly VoucherTypeIndicator CloseTemporaryAccounts = new(26, "بستن حسابهای موقت");
        /// <summary>
        /// بستن حسابهای عملکرد
        /// </summary>
        public static readonly VoucherTypeIndicator CloseOperatingAccounts = new(27, "بستن حسابهای عملکرد");
        /// <summary>
        /// محاسبات ارزشیابی و تنزیلات
        /// </summary>
        public static readonly VoucherTypeIndicator EvaluationDiscountingCalculations = new(28, "محاسبات ارزشیابی و تنزیلات");
        /// <summary>
        /// هزینه ارکان
        /// </summary>
        public static readonly VoucherTypeIndicator PillarCost = new(29, "هزینه ارکان");
        /// <summary>
        /// محاسبات درآمد و سایر هزینهها
        /// </summary>
        public static readonly VoucherTypeIndicator IncomeAndOtherExpensesCalculations = new(30, "محاسبات درآمد و سایر هزینهها");
        /// <summary>
        /// تعدیل روزانه کارمزد کارگزار
        /// </summary>
        public static readonly VoucherTypeIndicator Type31 = new(31, "تعدیل روزانه کارمزد کارگزار");
        /// <summary>
        /// کارمزد مدیر مبتنی بر عملکرد
        /// </summary>
        public static readonly VoucherTypeIndicator Type32 = new(32, "کارمزد مدیر مبتنی بر عملکرد");
        /// <summary>
        /// تقسیم سود واحدهای ممتاز
        /// </summary>
        public static readonly VoucherTypeIndicator Type33 = new(33, "تقسیم سود واحدهای ممتاز");
        /// <summary>
        /// سود روزانه اوراق
        /// </summary>
        public static readonly VoucherTypeIndicator BondDailyInterest = new(34, "سود روزانه اوراق");
        /// <summary>
        /// انتقال وجه فی مابین حساب بانک و کارگزار
        /// </summary>
        public static readonly VoucherTypeIndicator Type35 = new(35, "انتقال وجه فی مابین حساب بانک و کارگزار");
        /// <summary>
        /// واریزی نامشخص
        /// </summary>
        public static readonly VoucherTypeIndicator Type36 = new(36, "واریزی نامشخص");
        /// <summary>
        /// دیگر هزینه های سایر
        /// </summary>
        public static readonly VoucherTypeIndicator Type37 = new(37, "دیگر هزینه های سایر");
        /// <summary>
        /// هزینه نرم افزار
        /// </summary>
        public static readonly VoucherTypeIndicator Type38 = new(38, "هزینه نرم افزار");
        /// <summary>
        /// تسویه ناشی از اعمال تبعی
        /// </summary>
        public static readonly VoucherTypeIndicator Type39 = new(39, "تسویه ناشی از اعمال تبعی");
        /// <summary>
        /// سند ارزشیابی قرارداد‌های آتی
        /// </summary>
        public static readonly VoucherTypeIndicator Type40 = new(40, "سند ارزشیابی قرارداد‌های آتی");
        /// <summary>
        /// تسویه روزانه قرارداد‌های آتی کالا
        /// </summary>
        public static readonly VoucherTypeIndicator Type41 = new(41, "تسویه روزانه قرارداد‌های آتی کالا");
        /// <summary>
        /// خرید و فروش قرار داد آتی
        /// </summary>
        public static readonly VoucherTypeIndicator Type42 = new(42, "خرید و فروش قرار داد آتی");
        /// <summary>
        /// اعمال اختیار معامله
        /// </summary>
        public static readonly VoucherTypeIndicator OptionExercised = new(43, "اعمال اختیار معامله");
        /// <summary>
        /// عدم اعمال اختیار معامله
        /// </summary>
        public static readonly VoucherTypeIndicator OptionNotExercised = new(44, "عدم اعمال اختیار معامله");
        /// <summary>
        /// دریافت سود اوراق
        /// </summary>
        public static readonly VoucherTypeIndicator BondReceiveInterest = new(45, "دریافت سود اوراق");
        /// <summary>
        /// هزینه انبارداری
        /// </summary>
        public static readonly VoucherTypeIndicator Type46 = new(46, "هزینه انبارداری");
        /// <summary>
        /// هزینه ی ثابت
        /// </summary>
        public static readonly VoucherTypeIndicator Type47 = new(47, "هزینه ی ثابت");
        /// <summary>
        /// ذخیره تغییرات ارزش سهام
        /// </summary>
        public static readonly VoucherTypeIndicator Type48 = new(48, "ذخیره تغییرات ارزش سهام");
        /// <summary>
        /// تقسیم ذخیره تغییرات ارزش
        /// </summary>
        public static readonly VoucherTypeIndicator Type49 = new(49, "تقسیم ذخیره تغییرات ارزش");
        /// <summary>
        /// جریمه کارمزد مدیر
        /// </summary>
        public static readonly VoucherTypeIndicator Type50 = new(50, "جریمه کارمزد مدیر");
        /// <summary>
        /// اصلاحیه مانده بدهکاران
        /// </summary>
        public static readonly VoucherTypeIndicator Type51 = new(51, "اصلاحیه مانده بدهکاران");
        /// <summary>
        /// تایید صدور بازارگردان
        /// </summary>
        public static readonly VoucherTypeIndicator Type52 = new(52, "تایید صدور بازارگردان");
        /// <summary>
        /// تایید ابطال بازارگردان
        /// </summary>
        public static readonly VoucherTypeIndicator Type53 = new(53, "تایید ابطال بازارگردان");
        /// <summary>
        /// تسویه نقدی اختیار
        /// </summary>
        public static readonly VoucherTypeIndicator OptionCashSettlement = new(54, "تسویه نقدی اختیار");
        /// <summary>
        /// تسویه فیزیکی اختیار
        /// </summary>
        public static readonly VoucherTypeIndicator OptionPhysicallSettlement = new(55, "تسویه فیزیکی اختیار");
        /// <summary>
        /// تعدیلات ناشی از  سقف کارمزد
        /// </summary>
        public static readonly VoucherTypeIndicator Type56 = new(56, "تعدیلات ناشی از  سقف کارمزد");
        /// <summary>
        /// نقل و انتقال وجه بین بانک و کارگزاری
        /// </summary>
        public static readonly VoucherTypeIndicator Type57 = new(57, "نقل و انتقال وجه بین بانک و کارگزاری");
        /// <summary>
        /// ابطال تبعی
        /// </summary>
        public static readonly VoucherTypeIndicator EmbeddedRedemption = new(58, "ابطال تبعی");

        private VoucherTypeIndicator(int id, string name) : base(id, name)
        {
        }

        public static implicit operator int(VoucherTypeIndicator indicator) => indicator.Id;
    }
}
