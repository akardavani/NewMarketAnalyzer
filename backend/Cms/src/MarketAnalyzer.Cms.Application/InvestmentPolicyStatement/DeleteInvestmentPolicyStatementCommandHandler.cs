using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class DeleteInvestmentPolicyStatementCommandHandler : IRequestHandler<DeleteInvestmentPolicyStatementCommand, bool>
    {
        private readonly IDbContext _dbContext;
        private readonly IFileStore _fileStore;

        public DeleteInvestmentPolicyStatementCommandHandler(IDbContext dbContext, IFileStore fileStore)
        {
            _dbContext = dbContext;
            _fileStore = fileStore;
        }

        public async Task<bool> HandleAsync(DeleteInvestmentPolicyStatementCommand request, CancellationToken cancellationToken = default)
        {
            var statute = await _dbContext
                .Set<InvestmentPolicyStatement>()
                .Where(e => e.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (statute is null)
            {
                throw new ClientExposedException($"این اساسنامه وجود ندارد.");
            }

            await _fileStore.RemoveAsync(new ReadFileRequest
            {
                FileId = statute.AttachmentId,
                OperatorId = 1
            }, cancellationToken);

            _dbContext.Set<InvestmentPolicyStatement>().Remove(statute);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}