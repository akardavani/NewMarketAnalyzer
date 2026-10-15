using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App = MarketAnalyzer.Membership.Domain.Application;

namespace MarketAnalyzer.Membership.Application
{
    internal class ApplicationsSelectOptionQueryHandler : IRequestHandler<ApplicationsSelectOptionQuery, IReadOnlyCollection<SelectOption>>
    {
        private readonly IDbContext _dbContext;

        public ApplicationsSelectOptionQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<IReadOnlyCollection<SelectOption>> HandleAsync(ApplicationsSelectOptionQuery request, CancellationToken cancellationToken = default)
        {
            return _dbContext
               .Set<App>()
               .Select(e => new SelectOption()
               {
                   Value = e.Id,
                   Text = e.DisplayName
               })
               .OrderBy(e => e.Text)
               .ToReadOnlyCollectionAsync(cancellationToken);
        }
    }
}
