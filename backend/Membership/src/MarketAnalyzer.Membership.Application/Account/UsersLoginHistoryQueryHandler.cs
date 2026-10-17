using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class UsersLoginHistoryQueryHandler : IRequestHandler<UsersLoginHistoryQuery, QueryResult<UsersLoginHistoryQueryResult>>
    {
        private readonly IDbContext _dbContext;

        public UsersLoginHistoryQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<UsersLoginHistoryQueryResult>> HandleAsync(UsersLoginHistoryQuery request, CancellationToken cancellationToken = default)
        {
            if (request.OrderBy.Count == 0)
            {
                request.OrderBy[nameof(UsersLoginHistoryQueryResult.EventDateTime)] = SortType.DESC;
            }

            var query = from user in _dbContext.Set<User>()
                        from history in user.LoginHistories
                        where (request.FromDate == null || DateOnly.FromDateTime(history.CreationTime) >= request.FromDate) &&
                              (request.ToDate == null || DateOnly.FromDateTime(history.CreationTime) <= request.ToDate) &&
                              (string.IsNullOrEmpty(request.UserName) || user.UserName == request.UserName) &&
                              (request.LoginType == null || history.LoginType == request.LoginType) &&
                              (string.IsNullOrEmpty(request.IpAddress) || history.IpAddress == request.IpAddress) &&
                              (string.IsNullOrEmpty(request.UserAgent) || history.UserAgent.Contains(request.UserAgent)) &&
                              (request.ApplicationId == null || history.Application.Id == request.ApplicationId) &&
                              (request.ResultId == null || history.LoginResult.Id == request.ResultId)
                        select new UsersLoginHistoryQueryResult()
                        {
                            Id = history.Id,
                            UserId = user.Id,
                            UserName = user.UserName,
                            FullName = user.FullName,
                            LoginType = history.LoginType == LoginType.OneTimePassword ? "ورود دوعاملی" : "رمز ایستا",
                            IpAddress = history.IpAddress,
                            UserAgent = history.UserAgent,
                            LoginResult = history.LoginResult.Name,
                            Application = history.Application.DisplayName,
                            EventDateTime = history.CreationTime
                        };

            return new QueryResult<UsersLoginHistoryQueryResult>()
            {
                ItemsCount = await query.LongCountAsync(cancellationToken),
                Rows = await query.Pagination(request).ToListAsync(cancellationToken)
            };
        }
    }
}
