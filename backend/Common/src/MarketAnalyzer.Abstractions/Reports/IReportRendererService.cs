using System.Threading.Tasks;
using System.Threading;

namespace MarketAnalyzer.Abstractions
{
    public interface IReportRendererService : IExcelRenderer
    {
        Task<ReportResult> RenderAsync(RenderReportRequest request, CancellationToken cancellationToken);
    }
}
