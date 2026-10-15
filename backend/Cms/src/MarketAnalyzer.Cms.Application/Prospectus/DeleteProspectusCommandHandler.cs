using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class DeleteProspectusCommandHandler : IRequestHandler<DeleteProspectusCommand, bool>
    {
        private readonly IDbContext _dbContext;
        private readonly IFileStore _fileStore;

        public DeleteProspectusCommandHandler(IDbContext dbContext, IFileStore fileStore)
        {
            _dbContext = dbContext;
            _fileStore = fileStore;
        }

        public async Task<bool> HandleAsync(DeleteProspectusCommand request, CancellationToken cancellationToken = default)
        {
            var prospectus = await _dbContext
                .Set<Prospectus>()
                .Where(e => e.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (prospectus is null)
            {
                throw new ClientExposedException($"این امیدنامه وجود ندارد.");
            }

            await _fileStore.RemoveAsync(new ReadFileRequest
            {
                FileId = prospectus.AttachmentId,
                OperatorId = 1
            }, cancellationToken);

            _dbContext.Set<Prospectus>().Remove(prospectus);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}