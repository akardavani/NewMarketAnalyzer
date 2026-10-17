using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class RoleQueryHandler : IRequestHandler<RoleQuery, RoleQueryResult>
    {
        private readonly IDbContext _dbContext;

        public RoleQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<RoleQueryResult> HandleAsync(RoleQuery request, CancellationToken cancellationToken = default)
        {
            return _dbContext
                .Set<Role>()
                .Select(e => new RoleQueryResult()
                {
                    Id = e.Id,
                    Name = e.Name,
                    DisplayName = e.DisplayName,
                    IsActive = e.IsActive,
                    Editable = e.Editable,
                    RowVersion = e.RowVersion
                })
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        }
    }
}
