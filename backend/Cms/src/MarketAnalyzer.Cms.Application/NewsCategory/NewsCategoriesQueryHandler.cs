using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class NewsCategoriesQueryHandler : IRequestHandler<NewsCategoriesQuery, QueryResult<NewsCategoriesQueryResult>>
    {
        private readonly IDbContext _dbContext;
        public NewsCategoriesQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<QueryResult<NewsCategoriesQueryResult>> HandleAsync(NewsCategoriesQuery request, CancellationToken cancellationToken = default)
        {
            return _dbContext
                    .Set<NewsCategory>()
                    .QueryAsync(request.QueryParam, e => new NewsCategoriesQueryResult
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Description = e.Description,
                    }, cancellationToken);
        }
    }
}


