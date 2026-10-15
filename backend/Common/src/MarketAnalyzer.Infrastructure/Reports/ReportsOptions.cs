using MarketAnalyzer.Abstractions;
using System.Collections.Generic;

namespace MarketAnalyzer.Infrastructure.Reports
{
    public class ReportsOptions
    {
        private readonly Dictionary<ReportFormat, string> _formatDirectoryMap;

        public ReportsOptions()
        {
            _formatDirectoryMap = new Dictionary<ReportFormat, string>()
            {
                { ReportFormat.Excel, "Excels" }
            };
        }

        public string LicenseKey { get; set; }
        public string BasePath { get; set; }
        public string CompanyName { get; set; }
        public string SharedDirectory { get; set; } = "Shared";
        public Dictionary<ReportFormat, string> FormatDirectoryMap => _formatDirectoryMap;
    }
}
