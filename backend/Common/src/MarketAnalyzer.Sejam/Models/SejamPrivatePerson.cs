using System;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.Sejam.Models
{
    public class SejamPrivatePerson
    {
        /// <summary>
        /// نام
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// نام پدر
        /// </summary>
        public string FatherName { get; set; }
        /// <summary>
        /// جنسیت
        /// </summary>

        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public SejamGender Gender { get; set; }
        /// <summary>
        /// بخش حروفی سری شناسنامه
        /// </summary>
        public string SeriShChar { get; set; }
        /// <summary>
        /// بخش عددی سری شناسنامه
        /// </summary>
        public string SeriSh { get; set; }
        /// <summary>
        /// سریال شناسنامه
        /// </summary>
        public string Serial { get; set; }
        /// <summary>
        /// شماره شناسنامه
        /// </summary>
        public string ShNumber { get; set; }
        /// <summary>
        /// تاریخ تولد
        /// </summary>
        public DateTime BirthDate { get; set; }
        /// <summary>
        /// محل صدور شناسنامه
        /// </summary>
        public string PlaceOfIssue { get; set; }
        /// <summary>
        /// محل تولد
        /// </summary>
        public string PlaceOfBirth { get; set; }
    }
}