using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Cms.Application
{
    public class ExportProspectusQuery : IRequest<ReportResult>
    {
        public required string AttachmentId { get; init; }
        public required int OperatorId { get; init; }
    }    
}
