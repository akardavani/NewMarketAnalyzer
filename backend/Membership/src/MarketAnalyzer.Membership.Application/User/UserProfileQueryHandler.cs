using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.Membership.Application
{
    internal class UserProfileQueryHandler : IRequestHandler<UserProfileQuery, UserProfileQueryResult>
    {
        private readonly IDbContext _dbContext;

        public UserProfileQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<UserProfileQueryResult> HandleAsync(UserProfileQuery request, CancellationToken cancellationToken = default)
        {
            return _dbContext
                .Set<User>()
                .Where(e => e.Id == request.Id)
                .Select(e => new UserProfileQueryResult()
                {
                    FullName = e.FullName,
                    LastLoginDate = e.LastLoginDate.Value
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
