using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class OnlineUsersQueryHandler : IRequestHandler<OnlineUsersQuery, QueryResult<OnlineUsersQueryResult>>
    {
        private readonly IDbContext _dbContext;
        private readonly ISessionManager _sessionManager;

        public OnlineUsersQueryHandler(IDbContext dbContext, ISessionManager sessionManager)
        {
            _dbContext = dbContext;
            _sessionManager = sessionManager;
        }

        public async Task<QueryResult<OnlineUsersQueryResult>> HandleAsync(OnlineUsersQuery request, CancellationToken cancellationToken = default)
        {
            var applications = await _dbContext
                .Set<Domain.Application>()
                .ToDictionaryAsync(e => e.Id, e => e.DisplayName, cancellationToken);

            var query = _sessionManager
                .GetSessions()
                .Where(e => (request.UserId == null || e.UserId == request.UserId) &&
                            (request.UserName == null || e.UserName == request.UserName) &&
                            (request.Ip == null || e.Ip == request.Ip) &&
                            (request.ApplicationId == null || e.ApplicationId == request.ApplicationId) &&
                            (request.FromDate == null || e.LastActivityTime >= request.FromDate) &&
                            (request.ToDate == null || e.LastActivityTime <= request.ToDate));

            var rows = query
                .Select(e => new OnlineUsersQueryResult()
                {
                    UserId = e.UserId,
                    UserName = e.UserName,
                    IpAddress = e.Ip,
                    UserAgent = e.UserAgent,
                    Application = applications[e.ApplicationId],
                    LastActivityTime = e.LastActivityTime
                })
                .OrderByDescending(e => e.LastActivityTime)
                .Skip(request.Page * request.Rows)
                .Take(request.Rows)
                .ToList();

            return new QueryResult<OnlineUsersQueryResult>()
            {
                Rows = rows,
                ItemsCount = query.Count()
            };
        }
    }
}