using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class UsersQueryHandler : IRequestHandler<UsersQuery, QueryResult<UsersQueryResult>>
    {
        private readonly IDbContext _dbContext;

        public UsersQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<UsersQueryResult>> HandleAsync(UsersQuery request, CancellationToken cancellationToken = default)
        {
            var query = from user in _dbContext.Set<User>()
                        where (request.UserName == null || user.UserName.Contains(request.UserName)) &&
                              (request.FullName == null || user.FullName.Contains(request.FullName)) &&
                              (request.PhoneNumber == null || user.PhoneNumber.Contains(request.PhoneNumber)) &&
                              (request.Email == null || user.Email.Contains(request.Email)) &&
                              (request.TwoFactorAuthentication == null || user.TwoFactorAuthentication == request.TwoFactorAuthentication) &&
                              (request.IsActive == null || user.IsActive == request.IsActive)
                        select new UsersQueryResult()
                        {
                            Id = user.Id,
                            FullName = user.FullName,
                            UserName = user.UserName,
                            PhoneNumber = user.PhoneNumber,
                            Email = user.Email,
                            TwoFactorAuthentication = user.TwoFactorAuthentication,
                            IsActive = user.IsActive,
                            Editable = true
                        };

            return new QueryResult<UsersQueryResult>()
            {
                ItemsCount = await query.LongCountAsync(cancellationToken),
                Rows = await query.Pagination(request).ToListAsync(cancellationToken)
            };
        }
    }
}
