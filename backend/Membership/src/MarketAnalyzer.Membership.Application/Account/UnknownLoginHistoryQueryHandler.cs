using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class UnknownLoginHistoryQueryHandler : IRequestHandler<UnknownLoginHistoryQuery, QueryResult<UnknownLoginHistoryQueryResult>>
    {
        private readonly IDbContext _dbContext;

        public UnknownLoginHistoryQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<UnknownLoginHistoryQueryResult>> HandleAsync(UnknownLoginHistoryQuery request, CancellationToken cancellationToken = default)
        {
            if (request.OrderBy.Count == 0)
            {
                request.OrderBy[nameof(UnknownLoginHistoryQueryResult.EventDateTime)] = SortType.DESC;
            }

            var query = from history in _dbContext.Set<UnknownLoginHistory>()
                        where (request.FromDate == null || history.CreationTime.ToDateOnly() >= request.ToDate) &&
                              (request.ToDate == null || history.CreationTime.ToDateOnly() <= request.ToDate) &&
                              (string.IsNullOrEmpty(request.UserName) || history.UserName == request.UserName) &&
                              (request.LoginType == null || history.LoginType == request.LoginType) &&
                              (string.IsNullOrEmpty(request.IpAddress) || history.IpAddress == request.IpAddress) &&
                              (string.IsNullOrEmpty(request.UserAgent) || history.UserAgent.Contains(request.UserAgent)) &&
                              (request.ApplicationId == null || history.Application.Id == request.ApplicationId)
                        select new UnknownLoginHistoryQueryResult()
                        {
                            UserName = history.UserName,
                            LoginType = history.LoginType == LoginType.OneTimePassword ? "ورود دوعاملی" : "رمز ایستا",
                            IpAddress = history.IpAddress,
                            UserAgent = history.UserAgent,
                            Application = history.Application.DisplayName,
                            EventDateTime = history.CreationTime
                        };

            return new QueryResult<UnknownLoginHistoryQueryResult>()
            {
                ItemsCount = await query.LongCountAsync(cancellationToken),
                Rows = await query.Pagination(request).ToListAsync(cancellationToken)
            };
        }
    }
}