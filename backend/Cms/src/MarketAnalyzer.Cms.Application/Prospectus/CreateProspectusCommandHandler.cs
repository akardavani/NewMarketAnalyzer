using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class CreateProspectusCommandHandler : IRequestHandler<CreateProspectusCommand, int>
    {
        private readonly IDbContext _dbContext;
        private readonly IFileStore _fileStore;

        public CreateProspectusCommandHandler(IDbContext dbContext, IFileStore fileStore)
        {
            _dbContext = dbContext;
            _fileStore = fileStore;
        }

        public async Task<int> HandleAsync(CreateProspectusCommand request, CancellationToken cancellation = default)
        {
            var attachmentId = await _fileStore.WriteAsync(new WriteFileRequest()
            {
                Content = Convert.FromBase64String(request.FileContent.Content),
                FileName = request.FileContent.Name,
                Purpose = FilePurpose.ProspectusChanges.Id,
                OperatorId = request.OperatorId,
            }, cancellation);

            var prospectus = Prospectus.Create(
                                        date: request.Date,
                                        description: request.Description,
                                        attachmentId: attachmentId);

            _dbContext.Set<Prospectus>().Add(prospectus);
            await _dbContext.SaveChangesAsync(cancellation);
            return prospectus.Id;
        }
    }
}