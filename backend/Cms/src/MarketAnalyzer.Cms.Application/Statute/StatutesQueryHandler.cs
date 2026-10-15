using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class StatutesQueryHandler : IRequestHandler<StatutesQuery, QueryResult<StatutesQueryResult>>
    {
        private readonly IDbContext _dbContext;

        public StatutesQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<StatutesQueryResult>> HandleAsync(StatutesQuery request, CancellationToken cancellationToken = default)
        {
            var query = _dbContext
                .Set<Statute>()
                .IgnoreQueryFilters()
                .Where(e => (request.FromDate == null || e.Date >= request.FromDate &&
                            (request.ToDate == null || e.Date <= request.ToDate)))
                .OrderByDescending(e => e.CreationTime);

            var rows = await query
                .Select(e =>
                new StatutesQueryResult()
                {
                    Id = e.Id,
                    Description = e.Description,
                    AttachmentId = e.AttachmentId,
                    Date = e.Date
                })
                .Pagination(request)
                .ToListAsync(cancellationToken);


            return new QueryResult<StatutesQueryResult>()
            {
                Rows = rows,
                ItemsCount = await query.LongCountAsync(cancellationToken)
            };
        }
    }
}
