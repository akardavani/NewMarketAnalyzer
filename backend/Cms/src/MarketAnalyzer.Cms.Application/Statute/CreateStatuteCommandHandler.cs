using Microsoft.AspNetCore.Diagnostics;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class CreateStatuteCommandHandler : IRequestHandler<CreateStatuteCommand, int>
    {
        private readonly IDbContext _dbContext;
        private readonly IFileStore _fileStore;

        public CreateStatuteCommandHandler(IDbContext dbContext, IFileStore fileStore)
        {
            _dbContext = dbContext;
            _fileStore = fileStore;
        }

        public async Task<int> HandleAsync(CreateStatuteCommand request, CancellationToken cancellationToken = default)
        {
            var attachmentId = await _fileStore.WriteAsync(new WriteFileRequest()
            {
                Content = Convert.FromBase64String(request.FileContent.Content),
                FileName = request.FileContent.Name,
                Purpose = FilePurpose.StatuteChanges.Id,
                OperatorId = request.OperatorId,
            });

            var statute = Statute.Create(
                                        date: request.Date,
                                        description: request.Description,
                                        attachmentId: attachmentId);

            _dbContext.Set<Statute>().Add(statute);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return statute.Id;
        }
    }
}