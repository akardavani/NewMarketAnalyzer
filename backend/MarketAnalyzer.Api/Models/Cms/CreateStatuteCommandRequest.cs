using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.Api.Models
{
    public class CreateStatuteCommandRequest
    {
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateOnly Date { get; set; }
        /// <summary>
        /// شرح
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// محتوای فایل اساسنامه
        /// </summary>
        public FileContent FileContent { get; set; }
    }
}
