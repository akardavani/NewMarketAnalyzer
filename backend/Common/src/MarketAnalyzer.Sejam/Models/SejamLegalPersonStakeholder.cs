using System;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.Sejam.Models
{
    public class SejamLegalPersonStakeholder
    {
        /// <summary>
        /// کد ملی
        /// </summary>
        public string UniqueIdentifier { get; set; }
        /// <summary>
        /// نوع ذینفع شخصیت حقوقی
        /// </summary>

        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public SejamLegalPersonStakeholderType Type { get; set; }
        /// <summary>
        /// سمت
        /// </summary>

        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public SejamPositionType PositionType { get; set; }
        /// <summary>
        /// تاریخ شروع دوره تصدی
        /// </summary>
        public DateTime? StartAt { get; set; }
        /// <summary>
        /// تاریخ پایان دوره تصدی
        /// </summary>
        public DateTime? EndAt { get; set; }
        /// <summary>
        /// فیلد مشخص کننده صاحب امضا بودن فرد
        /// </summary>
        public bool IsOwnerSignature { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string LastName { get; set; }

    }
}