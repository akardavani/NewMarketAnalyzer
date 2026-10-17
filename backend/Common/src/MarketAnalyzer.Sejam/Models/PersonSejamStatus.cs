using System.Text.Json.Serialization;

namespace MarketAnalyzer.Sejam.Models
{
    /// <summary>
    /// وضعیت شخص در سجام
    /// </summary>
    public class PersonSejamStatus
    {
        /// <summary>
        /// وضعیت
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SejamUserProfileStatus Status { get; set; }
        /// <summary>
        /// در حالت سجامی ناقص نیاز به احراز هویت دارد یا خیر
        /// </summary>
        public bool? IsAuthenticate { get; set; }
        /// <summary>
        /// دلیل سجامی ناقص
        /// </summary>
        public string Reason { get; set; }
    }
}