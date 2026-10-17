using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface IExcelRenderer
    {
        Task<ReportResult> RenderAsync(string filePath, Func<IExcelDocument, Task<byte[]>> action, CancellationToken cancellation);
    }
}
