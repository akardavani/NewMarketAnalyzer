using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.Api.Models
{
    public class CreateProspectusCommandRequest
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
        /// محتوای فایل امیدنامه
        /// </summary>
        public FileContent FileContent { get; set; }
    }
}
