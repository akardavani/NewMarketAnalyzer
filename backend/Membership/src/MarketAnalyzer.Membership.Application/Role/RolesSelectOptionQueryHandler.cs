using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class RolesSelectOptionQueryHandler : IRequestHandler<RolesSelectOptionQuery, List<SelectOption>>
    {
        private readonly IDbContext _dbContext;

        public RolesSelectOptionQueryHandler(IDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public Task<List<SelectOption>> HandleAsync(RolesSelectOptionQuery request, CancellationToken cancellationToken = default)
        {
            return _dbContext
               .Set<Role>()
               .Select(e => new SelectOption()
               {
                   Value = e.Id,
                   Text = e.DisplayName
               })
               .OrderBy(e => e.Text)
               .ToListAsync(cancellationToken);
        }
    }
}
