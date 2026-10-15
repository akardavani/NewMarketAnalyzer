using MarketAnalyzer.Abstractions;
using System.Linq;

namespace MarketAnalyzer.SharedKernel
{
    public class FilePurposeIndicator : Enumeration
    {
        /// <summary>
        /// روی کارت ملی
        /// </summary>
        public static readonly FilePurposeIndicator FrontNationalCard = new(1, "روی کارت ملی");
        /// <summary>
        /// پشت کارت ملی
        /// </summary>
        public static readonly FilePurposeIndicator BackNationalCard = new(2, "پشت کارت ملی");
        /// <summary>
        /// صفحه اول شناسنامه
        /// </summary>
        public static readonly FilePurposeIndicator BirthCertificateFirstPage = new(3, "صفحه اول شناسنامه");
        /// <summary>
        /// صفحه دوم شناسنامه
        /// </summary>
        public static readonly FilePurposeIndicator BirthCertificateSecondPage = new(4, "صفحه دوم شناسنامه");
        /// <summary>
        /// صفحه سوم شناسنامه
        /// </summary>
        public static readonly FilePurposeIndicator BirthCertificateThirdPage = new(5, "صفحه سوم شناسنامه");
        /// <summary>
        /// گذرنامه
        /// </summary>
        public static readonly FilePurposeIndicator Passport = new(6, "گذرنامه");
        /// <summary>
        /// امضای سرمایه گذار
        /// </summary>
        public static readonly FilePurposeIndicator InvestorSignature = new(7, "امضای سرمایه گذار");
        /// <summary>
        /// تغییرات اساسنامه
        /// </summary>
        public static readonly FilePurposeIndicator StatuteChanges = new(8, "تغییرات اساسنامه");
        /// <summary>
        /// تغییرات امیدنامه
        /// </summary>
        public static readonly FilePurposeIndicator ProspectusChanges = new(9, "تغییرات امیدنامه");
        /// <summary>
        /// فیش واریزی
        /// </summary>
        public static readonly FilePurposeIndicator DepositSlip = new(10, "فیش واریزی");
        /// <summary>
        /// فایل معاملات TCK
        /// </summary>
        public static readonly FilePurposeIndicator TckTradeFile = new(11, "فایل معاملات TCK");
        /// <summary>
        /// فایل معاملات DBS
        /// </summary>
        public static readonly FilePurposeIndicator DbsTradeFile = new(12, "فایل معاملات DBS");
        /// <summary>
        /// تعدیل قیمت سهام
        /// </summary>
        public static readonly FilePurposeIndicator StockPriceAdjustment = new(13, "تعدیل قیمت سهام");
        /// <summary>
        /// پیوست اخبار
        /// </summary>
        public static readonly FilePurposeIndicator NewsAttachment = new(14, "پیوست اخبار");
        /// <summary>
        /// فایل رزومه ارکان
        /// </summary>
        public static readonly FilePurposeIndicator PillarRole = new(15, "فایل رزومه ارکان");
        /// <summary>
        /// فایل بیانیه سیاست های سرمایه گذاری
        /// </summary>
        public static readonly FilePurposeIndicator InvestmentPolicyStatement = new(16, "بیانیه سیاست های سرمایه گذاری");

        private FilePurposeIndicator(int id, string name) : base(id, name)
        {
        }

        public static explicit operator FilePurposeIndicator(int id) =>
            GetValues<FilePurposeIndicator>().First(e => e.Id == id);
    }
}
