using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class UserQueryHandler : IRequestHandler<UserQuery, UserQueryResult>
    {
        private readonly IDbContext _dbContext;

        public UserQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<UserQueryResult> HandleAsync(UserQuery request, CancellationToken cancellationToken = default)
        {
            return _dbContext
                 .Set<User>()
                 .Select(e => new UserQueryResult()
                 {
                     Id = e.Id,
                     FullName = e.FullName,
                     UserName = e.UserName,
                     PhoneNumber = e.PhoneNumber,
                     Email = e.Email,
                     TwoFactorAuthentication = e.TwoFactorAuthentication,
                     Roles = e.Roles.Select(e => e.Id).ToList(),
                     IpAddresses = string.Join(";", e.IpAddresses.Select(e => e.Value).ToList())
                 })
                 .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        }
    }
}
