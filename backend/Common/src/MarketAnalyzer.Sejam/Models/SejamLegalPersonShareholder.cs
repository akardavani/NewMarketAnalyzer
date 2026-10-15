using System.Text.Json.Serialization;

namespace MarketAnalyzer.Sejam.Models
{
    public class SejamLegalPersonShareholder
    {
        /// <summary>
        /// کد ملی
        /// </summary>
        public string UniqueIdentifier { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// کد پستی
        /// </summary>
        public string PostalCode { get; set; }
        /// <summary>
        /// نشانی کامل
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// سمت
        /// </summary>

        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public SejamPositionType PositionType { get; set; }
        /// <summary>
        /// درصد سهامداری
        /// </summary>
        public int PercentageVotingRight { get; set; }
    }
}