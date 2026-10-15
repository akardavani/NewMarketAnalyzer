using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class PortalStatutesQueryHandler : IRequestHandler<PortalStatutesQuery, QueryResult<PortalStatutesQueryResult>>
    {
        private readonly IDbContext _dbContext;

        public PortalStatutesQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<PortalStatutesQueryResult>> HandleAsync(PortalStatutesQuery request, CancellationToken cancellationToken = default)
        {

            var query = _dbContext
                .Set<Statute>()
                .IgnoreQueryFilters()
                .OrderByDescending(e => e.Date);

            if (!query.Any())
            {
                throw new ClientExposedException("داده‌ای برای نمایش وجود ندارد.");
            }

            var rows = await query
                .Select(e =>
                new PortalStatutesQueryResult()
                {
                    Id = e.Id,
                    Date = e.Date,
                    Description = e.Description,
                    AttachmentId = e.AttachmentId,
                })
                .ToListAsync(cancellationToken);

            return new QueryResult<PortalStatutesQueryResult>()
            {
                Rows = rows,
                ItemsCount = await query.LongCountAsync(cancellationToken)
            };
        }
    }
}
