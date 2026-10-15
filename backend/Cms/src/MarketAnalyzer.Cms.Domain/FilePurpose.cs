using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Cms.Domain
{
    /// <summary>
    /// موضوع فایل مثلا: کارت ملی، شناسنامه و غیره
    /// </summary>
    public class FilePurpose : AuditableEntity<int>
    {
        /// <summary>
        /// روی کارت ملی
        /// </summary>
        public static readonly FilePurpose FrontNationalCard = new(1) { Name = "روی کارت ملی" };
        /// <summary>
        /// پشت کارت ملی
        /// </summary>
        public static readonly FilePurpose BackNationalCard = new(2) { Name = "پشت کارت ملی" };
        /// <summary>
        /// صفحه اول شناسنامه
        /// </summary>
        public static readonly FilePurpose BirthCertificateFirstPage = new(3) { Name = "صفحه اول شناسنامه" };
        /// <summary>
        /// صفحه دوم شناسنامه
        /// </summary>
        public static readonly FilePurpose BirthCertificateSecondPage = new(4) { Name = "صفحه دوم شناسنامه" };
        /// <summary>
        /// صفحه سوم شناسنامه
        /// </summary>
        public static readonly FilePurpose BirthCertificateThirdPage = new(5) { Name = "صفحه سوم شناسنامه" };
        /// <summary>
        /// گذرنامه
        /// </summary>
        public static readonly FilePurpose Passport = new(6) { Name = "گذرنامه" };
        /// <summary>
        /// امضای سرمایه گذار
        /// </summary>
        public static readonly FilePurpose InvestorSignature = new(7) { Name = "امضای سرمایه گذار" };
        /// <summary>
        /// تغییرات اساسنامه
        /// </summary>
        public static readonly FilePurpose StatuteChanges = new(8) { Name = "تغییرات اساسنامه" };
        /// <summary>
        /// تغییرات امیدنامه
        /// </summary>
        public static readonly FilePurpose ProspectusChanges = new(9) { Name = "تغییرات امیدنامه" };
        /// <summary>
        /// فیش واریزی
        /// </summary>
        public static readonly FilePurpose DepositSlip = new(10) { Name = "فیش واریزی" };
        /// <summary>
        /// فایل معاملات TCK
        /// </summary>
        public static readonly FilePurpose TckTradeFile = new(11) { Name = "فایل معاملات TCK" };
        /// <summary>
        /// فایل معاملات DBS
        /// </summary>
        public static readonly FilePurpose DbsTradeFile = new(12) { Name = "فایل معاملات DBS" };
        /// <summary>
        /// تعدیل قیمت سهام
        /// </summary>
        public static readonly FilePurpose StockPriceAdjustment = new(13) { Name = "تعدیل قیمت سهام" };
        /// <summary>
        /// پیوست اخبار
        /// </summary>
        public static readonly FilePurpose NewsAttachment = new(14) { Name = "پیوست اخبار" };
        /// <summary>
        /// فایل رزومه ارکان
        /// </summary>
        public static readonly FilePurpose PillarRole = new(15) { Name = "فایل رزومه ارکان" };
        /// <summary>
        /// فایل بیانیه سیاست های سرمایه گذاری
        /// </summary>
        public static readonly FilePurpose InvestmentPolicyStatement = new(16) { Name = "بیانیه سیاست های سرمایه گذاری" };
        /// <summary>
        /// فایل پیوست حسابهای بانکی صندوق
        /// </summary>
        public static readonly FilePurpose CompanyBankAccount = new(17) { Name = "حسابهای بانکی صندوق" };

        public FilePurpose(int id) : base(id)
        {
        }

        /// <summary>
        /// نام موضوع
        /// </summary>
        public string Name { get; private set; }

        private List<FilePurposeAcceptableType> _acceptableTypes;
        public IReadOnlyCollection<FilePurposeAcceptableType> AcceptableTypes => _acceptableTypes ??= new List<FilePurposeAcceptableType>();


        public void Update(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            Name = name;
        }

        public void AddFileType(FileType fileType, long maxSize)
        {
            if (AcceptableTypes.Any(e => e.FileType == fileType))
            {
                throw new Exception("Duplicate file type.");
            }

            _acceptableTypes.Add(FilePurposeAcceptableType.Create(fileType: fileType, maxSize: maxSize));
        }

        public void RemoveFileType(FileType fileType)
        {
            if (fileType is null)
            {
                throw new ArgumentNullException(nameof(fileType));
            }

            var entry = AcceptableTypes.FirstOrDefault(e => e.FileType == fileType);
            if (entry is null)
            {
                throw new Exception("The entry not exists");
            }
            _acceptableTypes.Remove(entry);
        }
    }
}
