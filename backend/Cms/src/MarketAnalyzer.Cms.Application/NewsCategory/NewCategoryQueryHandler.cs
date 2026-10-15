using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application;
internal class NewCategoryQueryHandler : IRequestHandler<NewsCategoryQuery, NewsCategoryQueryResult>
{
    private readonly IDbContext _dbContext;

    public NewCategoryQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<NewsCategoryQueryResult> HandleAsync(NewsCategoryQuery request, CancellationToken cancellationToken = default)
    {
        return _dbContext
        .Set<NewsCategory>()
        .Select(e => new NewsCategoryQueryResult()
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
        })
        .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
    }
}

