using MarketAnalyzer.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class ExportInvestmentPolicyStatementsQueryHandler : IRequestHandler<ExportInvestmentPolicyStatementsQuery, ReportResult>
    {
        private readonly IFileStore _fileStore;

        public ExportInvestmentPolicyStatementsQueryHandler(IFileStore fileStore)
        {
            _fileStore = fileStore;
        }

        public async Task<ReportResult> HandleAsync(ExportInvestmentPolicyStatementsQuery request, CancellationToken cancellationToken = default)
        {
            var resumeFile = await _fileStore.ReadAsync(new ReadFileRequest
            {
                FileId = request.AttachmentId,
                OperatorId = request.OperatorId
            }, cancellationToken);

            return new ReportResult
            {
                ContentType = resumeFile.MimeType,
                Data = resumeFile.Content,
                FileName = resumeFile.FileName
            };
        }
    }
}
