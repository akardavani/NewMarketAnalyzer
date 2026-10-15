using System;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.Sejam.Models
{
    public class SejamLegalPerson
    {
        /// <summary>
        /// نام شخص حقوقی
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// شماره ثبت شخص حقوقی
        /// </summary>
        public string RegisterNumber { get; set; }
        /// <summary>
        /// محل ثبت
        /// </summary>
        public string RegisterPlace { get; set; }
        /// <summary>
        /// تاریخ ثبت
        /// </summary>
        public DateTime RegisterDate { get; set; }
        /// <summary>
        /// کد اقتصادی
        /// </summary>
        public string EconomicCode { get; set; }
        /// <summary>
        /// سازمان صادرکننده مجوز
        /// </summary>
        public string EvidenceReleaseCompany { get; set; }
        /// <summary>
        /// تاریخ صدور مجوز
        /// </summary>
        public DateTime? EvidenceReleaseDate { get; set; }
        /// <summary>
        /// تاریخ انقضاء مجوز
        /// </summary>
        public DateTime? EvidenceExpirationDate { get; set; }
        /// <summary>
        /// نوع شرکت[
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SejamLegalPersonType? LegalPersonTypeCategory { get; set; }
        /// <summary>
        /// زیرمجموعه نوع شرکت
        /// </summary>[
        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public SejamLegalPersonTypeSubCategory? LegalPersonTypeSubCategory { get; set; }
    }
}