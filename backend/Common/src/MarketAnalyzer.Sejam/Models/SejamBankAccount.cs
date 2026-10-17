using System.Text.Json.Serialization;

namespace MarketAnalyzer.Sejam.Models
{
    public class SejamBankAccount
    {
        /// <summary>
        /// شماره حساب
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// نوع حساب
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SejamBankAccountType Type { get; set; }
        /// <summary>
        /// شماره شبا
        /// </summary>
        public string Sheba { get; set; }
        /// <summary>
        /// نام بانک
        /// </summary>
        public SejamEntry Bank { get; set; }
        /// <summary>
        /// کد شعبه
        /// </summary>
        public string BranchCode { get; set; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public string BranchName { get; set; }
        /// <summary>
        /// شهر شعبه بانک
        /// </summary>
        public SejamRegion BranchCity { get; set; }
        /// <summary>
        /// مشخص کننده شماره حساب پیش فرض
        /// </summary>
        public bool IsDefault { get; set; }
    }
}