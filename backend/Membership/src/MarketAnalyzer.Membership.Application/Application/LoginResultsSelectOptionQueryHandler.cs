using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class LoginResultsSelectOptionQueryHandler : IRequestHandler<LoginResultsSelectOptionQuery, IReadOnlyCollection<SelectOption>>
    {
        private readonly IDbContext _dbContext;

        public LoginResultsSelectOptionQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<IReadOnlyCollection<SelectOption>> HandleAsync(LoginResultsSelectOptionQuery request, CancellationToken cancellationToken = default)
        {
            return _dbContext
               .Set<LoginResult>()
               .Select(e => new SelectOption()
               {
                   Value = e.Id,
                   Text = e.Name
               })
               .OrderBy(e => e.Text)
               .ToReadOnlyCollectionAsync(cancellationToken);
        }
    }
}
