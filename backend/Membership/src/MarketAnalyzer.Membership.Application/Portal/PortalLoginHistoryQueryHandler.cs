using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class PortalLoginHistoryQueryHandler : IRequestHandler<PortalLoginHistoryQuery, QueryResult<PortalLoginHistoryQueryResult>>
    {
        private readonly IDbContext _dbContext;

        public PortalLoginHistoryQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<PortalLoginHistoryQueryResult>> HandleAsync(PortalLoginHistoryQuery request, CancellationToken cancellationToken = default)
        {
            if (request.OrderBy.Count == 0)
            {
                request.OrderBy[nameof(PortalLoginHistoryQueryResult.EventDateTime)] = SortType.DESC;
            }

            var query = _dbContext
                .Set<User>()
                .SelectMany(e => e.LoginHistories, (user, history) => new { user, history })
                .Where(e => e.user.Id == request.UserId);

            var rows = await query
                .Select(e => new PortalLoginHistoryQueryResult()
                {
                    IpAddress = e.history.IpAddress,
                    EventDateTime = e.history.CreationTime,
                    LoginResult = e.history.LoginResult.Name,
                    Application = e.history.Application.DisplayName,
                    Description = null
                })
                .Pagination(request)
                .ToListAsync(cancellationToken);

            return new QueryResult<PortalLoginHistoryQueryResult>()
            {
                Rows = rows,
                ItemsCount = await query.LongCountAsync(cancellationToken),
            };
        }
    }
}
