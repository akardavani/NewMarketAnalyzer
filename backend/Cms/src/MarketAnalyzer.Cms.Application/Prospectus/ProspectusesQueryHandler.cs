using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class ProspectusesQueryHandler : IRequestHandler<ProspectusesQuery, QueryResult<ProspectusesQueryResult>>
    {
        private readonly IDbContext _dbContext;

        public ProspectusesQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<ProspectusesQueryResult>> HandleAsync(ProspectusesQuery request, CancellationToken cancellationToken = default)
        {

            var query = _dbContext
                .Set<Prospectus>()
                .IgnoreQueryFilters()
                .Where(e => (request.FromDate == null || e.Date >= request.FromDate) &&
                            (request.ToDate == null || e.Date <= request.ToDate))
                .OrderByDescending(e => e.CreationTime);

            var rows = await query
                .Select(e =>
                new ProspectusesQueryResult()
                {
                    Id = e.Id,
                    Date = e.Date,
                    Description = e.Description,
                    AttachmentId = e.AttachmentId,
                })
                .Pagination(request)
                .ToListAsync(cancellationToken);


            return new QueryResult<ProspectusesQueryResult>()
            {
                Rows = rows,
                ItemsCount = await query.LongCountAsync(cancellationToken)
            };
        }
    }
}
