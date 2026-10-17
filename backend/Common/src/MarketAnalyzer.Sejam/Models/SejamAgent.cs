using System;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.Sejam.Models
{
    public class SejamAgent
    {
        /// <summary>
        /// مشخص کننده نوع نماینده
        /// </summary>

        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public SejamAgentType Type { get; set; }
        /// <summary>
        /// تاریخ انقضای نمایندگی
        /// </summary>
        public DateTime ExpirationDate { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string Description { get; set; }
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
        /// مشخص کننده تایید نماینده
        /// </summary>
        public bool IsConfirmed { get; set; }
    }
}