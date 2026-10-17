using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class RolesQueryHandler : IRequestHandler<RolesQuery, QueryResult<RolesQueryResult>>
    {
        private readonly IDbContext _dbContext;

        public RolesQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<RolesQueryResult>> HandleAsync(RolesQuery request, CancellationToken cancellationToken = default)
        {
            var query = from role in _dbContext.Set<Role>()
                        where (string.IsNullOrEmpty(request.Name) || role.Name.Contains(request.Name)) &&
                              ((string.IsNullOrEmpty(request.DisplayName) || role.DisplayName.Contains(request.DisplayName)) &&
                              (request.IsActive == null || role.IsActive == request.IsActive))
                        select new RolesQueryResult()
                        {
                            Id = role.Id,
                            Name = role.Name,
                            DisplayName = role.DisplayName,
                            IsActive = role.IsActive,
                            Editable = role.Editable,
                            UsersCount = role.Users.Count()
                        };

            return new QueryResult<RolesQueryResult>()
            {
                ItemsCount = await query.LongCountAsync(cancellationToken),
                Rows = await query.Pagination(request).ToListAsync(cancellationToken)
            };
        }
    }
}
