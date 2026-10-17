using System.Collections.Generic;

namespace MarketAnalyzer.Abstractions
{
    public class RenderReportRequest
    {
        public required object Data { get; init; }
        public required string FileName { get; init; }
        /// <summary>
        /// نام فایل جهت دانلود میتواند نال باشد
        /// </summary>
        public string DownloadFileName { get; init; }
        public required ReportFormat Format { get; init; }
        public required IReadOnlyDictionary<string, object> Variables { get; init; }
    }    
}
