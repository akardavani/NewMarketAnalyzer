using MarketAnalyzer.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class ExportStatutesQueryHandler : IRequestHandler<ExportStatutesQuery, ReportResult>
    {
        private readonly IFileStore _fileStore;

        public ExportStatutesQueryHandler(IFileStore fileStore)
        {
            _fileStore = fileStore;
        }

        public async Task<ReportResult> HandleAsync(ExportStatutesQuery request, CancellationToken cancellationToken = default)
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
