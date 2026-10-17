using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class InvestmentPolicyStatementsQueryHandler : IRequestHandler<InvestmentPolicyStatementsQuery, QueryResult<InvestmentPolicyStatementsQueryResult>>
    {
        private readonly IDbContext _dbContext;

        public InvestmentPolicyStatementsQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<InvestmentPolicyStatementsQueryResult>> HandleAsync(InvestmentPolicyStatementsQuery request, CancellationToken cancellationToken = default)
        {

            var query = _dbContext
                .Set<InvestmentPolicyStatement>()
                .IgnoreQueryFilters()
                .Where(e => (request.FromDate == null || e.Date >= request.FromDate) &&
                            (request.ToDate == null || e.Date <= request.ToDate) &&
                            (request.IsActive == null || e.IsActive == request.IsActive))
                .OrderByDescending(e => e.CreationTime);

            var rows = await query
                .Select(e =>
                new InvestmentPolicyStatementsQueryResult()
                {
                    Id = e.Id,
                    Date = e.Date,
                    Description = e.Description,
                    IsActive = e.IsActive,
                    AttachmentId = e.AttachmentId
                })
                .Pagination(request)
                .ToListAsync(cancellationToken);


            return new QueryResult<InvestmentPolicyStatementsQueryResult>()
            {
                Rows = rows,
                ItemsCount = await query.LongCountAsync(cancellationToken)
            };
        }
    }
}
