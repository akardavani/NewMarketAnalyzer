using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.Sejam.Models
{
    public class SejamPerson
    {
        /// <summary>
        /// کد ملی
        /// </summary>
        public string UniqueIdentifier { get; set; }
        /// <summary>
        /// شماره تلفن همراه شخص
        /// </summary>
        public long Mobile { get; set; }
        /// <summary>
        /// پست الکترونیکی
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// نوع شخص
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SejamPersonType Type { get; set; }
        /// <summary>
        /// وضعیت شخص در سجام
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SejamUserProfileStatus Status { get; set; }
        /// <summary>
        /// اطلاعات هویتی شخص حقیقی
        /// </summary>
        public SejamPrivatePerson PrivatePerson { get; set; }
        /// <summary>
        /// اطلاعات هویتی شخص حقوقی
        /// </summary>
        public SejamLegalPerson LegalPerson { get; set; }
        /// <summary>
        /// اطلاعات ارتباطی
        /// </summary>
        public List<SejamAddress> Addresses { get; set; }
        /// <summary>
        /// اطلاعات کدهای سهامداری
        /// </summary>
        public List<SejamTradingCode> TradingCodes { get; set; }
        /// <summary>
        /// نماینده شخص حقیقی
        /// </summary>
        public SejamAgent Agent { get; set; }
        /// <summary>
        /// اطلاعات حسابهای بانکی
        /// </summary>
        public List<SejamBankAccount> Accounts { get; set; }
        /// <summary>
        /// اطلاعات شغلی
        /// </summary>
        public SejamJobInfo JobInfo { get; set; }
        /// <summary>
        /// اطلاعات مالی
        /// </summary>
        public SejamFinancialInfo FinancialInfo { get; set; }
        /// <summary>
        /// اطلاعات ذینفعان شخص حقوقی
        /// </summary>
        public List<SejamLegalPersonShareholder> LegalPersonShareholders { get; set; }
        /// <summary>
        /// اطلاعات سهامداران شخص حقوقی
        /// </summary>
        public List<SejamLegalPersonStakeholder> LegalPersonStakeholders { get; set; }
    }
}