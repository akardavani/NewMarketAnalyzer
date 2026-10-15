using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class PortalProspectusesQueryHandler : IRequestHandler<PortalProspectusesQuery, QueryResult<PortalProspectusesQueryResult>>
    {
        private readonly IDbContext _dbContext;

        public PortalProspectusesQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<PortalProspectusesQueryResult>> HandleAsync(PortalProspectusesQuery request, CancellationToken cancellationToken = default)
        {

            var query = _dbContext
                .Set<Prospectus>()
                .IgnoreQueryFilters()
                .OrderByDescending(e => e.Date);

            if (!query.Any())
            {
                throw new ClientExposedException("داده‌ای برای نمایش وجود ندارد.");
            }

            var rows = await query
                .Select(e =>
                new PortalProspectusesQueryResult()
                {
                    Id = e.Id,
                    Date = e.Date,
                    Description = e.Description,
                    AttachmentId = e.AttachmentId,
                })
                .ToListAsync(cancellationToken);


            return new QueryResult<PortalProspectusesQueryResult>()
            {
                Rows = rows,
                ItemsCount = await query.LongCountAsync(cancellationToken)
            };
        }
    }
}
