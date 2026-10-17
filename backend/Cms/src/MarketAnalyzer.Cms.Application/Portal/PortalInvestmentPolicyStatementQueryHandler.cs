using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class PortalInvestmentPolicyStatementQueryHandler : IRequestHandler<PortalInvestmentPolicyStatementQuery, QueryResult<PortalInvestmentPolicyStatementQueryResult>>
    {
        private readonly IDbContext _dbContext;

        public PortalInvestmentPolicyStatementQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<PortalInvestmentPolicyStatementQueryResult>> HandleAsync(PortalInvestmentPolicyStatementQuery request, CancellationToken cancellationToken = default)
        {

            var query = _dbContext
                .Set<InvestmentPolicyStatement>()
                .IgnoreQueryFilters()
                .OrderByDescending(e => e.CreationTime);

            var rows = await query
                .Select(e =>
                new PortalInvestmentPolicyStatementQueryResult()
                {
                    Id = e.Id,
                    Date = e.Date,
                    Description = e.Description,
                    AttachmentId = e.AttachmentId,
                })
                .ToListAsync(cancellationToken);


            return new QueryResult<PortalInvestmentPolicyStatementQueryResult>()
            {
                Rows = rows,
                ItemsCount = await query.LongCountAsync(cancellationToken)
            };
        }
    }
}
