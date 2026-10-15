using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class LoginHistoryQueryHandler : IRequestHandler<LoginHistoryQuery, QueryResult<LoginHistoryQueryResult>>
    {
        private readonly IDbContext _dbContext;

        public LoginHistoryQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<LoginHistoryQueryResult>> HandleAsync(LoginHistoryQuery request, CancellationToken cancellationToken = default)
        {
            if (request.OrderBy.Count == 0)
            {
                request.OrderBy[nameof(UsersLoginHistoryQueryResult.EventDateTime)] = SortType.DESC;
            }

            var query = from user in _dbContext.Set<User>()
                        from history in user.LoginHistories
                        where (request.FromDate == null || DateOnly.FromDateTime(history.CreationTime) >= request.ToDate) &&
                              (request.ToDate == null || DateOnly.FromDateTime(history.CreationTime) <= request.ToDate) &&
                              (request.LoginType == null || history.LoginType == request.LoginType) &&
                              (string.IsNullOrEmpty(request.IpAddress) || history.IpAddress == request.IpAddress) &&
                              (string.IsNullOrEmpty(request.UserAgent) || history.UserAgent.Contains(request.UserAgent)) &&
                              (request.ApplicationId == null || history.Application.Id == request.ApplicationId) &&
                              (request.ResultId == null || history.LoginResult.Id == request.ResultId)
                        select new LoginHistoryQueryResult()
                        {
                            Id = history.Id,
                            LoginType = history.LoginType == LoginType.OneTimePassword ? "ورود دوعاملی" : "رمز ایستا",
                            IpAddress = history.IpAddress,
                            UserAgent = history.UserAgent,
                            LoginResult = history.LoginResult.Name,
                            Application = history.Application.DisplayName,
                            EventDateTime = history.CreationTime
                        };

            return new QueryResult<LoginHistoryQueryResult>()
            {
                ItemsCount = await query.LongCountAsync(cancellationToken),
                Rows = await query.Pagination(request).ToListAsync(cancellationToken)
            };
        }
    }
}
