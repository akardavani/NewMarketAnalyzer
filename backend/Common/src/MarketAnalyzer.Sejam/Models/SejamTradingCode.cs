using System.Text.Json.Serialization;

namespace MarketAnalyzer.Sejam.Models
{
    public class SejamTradingCode
    {
        /// <summary>
        /// نوع کد بورسی
        /// </summary>

        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public SejamTradingCodeType Type { get; set; }
        /// <summary>
        /// بخش اول کد سهامداری
        /// </summary>
        public string FirstPart { get; set; }
        /// <summary>
        /// بخش دوم کد سهامداری
        /// </summary>
        public string SecondtPart { get; set; }
        /// <summary>
        /// بخش سوم کد سهامداری
        /// </summary>
        public string ThirdPart { get; set; }
        /// <summary>
        /// کد سهامداری بصورت کامل
        /// </summary>
        public string Code { get; set; }
    }
}