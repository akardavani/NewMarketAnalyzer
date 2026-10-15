using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    /// <summary>
    /// سازمان XML سرویس ایجاد
    /// </summary>
    public interface IFundXmlReportGenerator
    {
        /// <summary>
        /// XML تولید 
        /// </summary>
        /// <param name="navDate">NAV تاریخ</param>
        Task<string> GenerateAsync(DateOnly navDate, CancellationToken cancellation = default);
    }
}
