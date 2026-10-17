using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class CreateInvestmentPolicyStatementCommandHandler : IRequestHandler<CreateInvestmentPolicyStatementCommand, int>
    {
        private readonly IDbContext _dbContext;
        private readonly IFileStore _fileStore;

        public CreateInvestmentPolicyStatementCommandHandler(IDbContext dbContext, IFileStore fileStore)
        {
            _dbContext = dbContext;
            _fileStore = fileStore;
        }

        public async Task<int> HandleAsync(CreateInvestmentPolicyStatementCommand request, CancellationToken cancellationToken = default)
        {
            var attachmentId = await _fileStore.WriteAsync(new WriteFileRequest()
            {
                Content = Convert.FromBase64String(request.FileContent.Content),
                FileName = request.FileContent.Name,
                Purpose = FilePurpose.InvestmentPolicyStatement.Id,
                OperatorId = request.OperatorId,
            });

            var investmentPolicyStatement = InvestmentPolicyStatement.Create(
                                        date: request.Date,
                                        description: request.Description,
                                        isActive: request.IsActive,
                                        attachmentId: attachmentId);

            _dbContext.Set<InvestmentPolicyStatement>().Add(investmentPolicyStatement);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return investmentPolicyStatement.Id;
        }
    }
}