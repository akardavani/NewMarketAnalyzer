using Microsoft.EntityFrameworkCore;
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
    public class UpdateNewsCategoryCommandHandler : IRequestHandler<UpdateNewsCategoryCommand, ApiResult<int>>
    {
        private readonly IDbContext _dbContext;

        public UpdateNewsCategoryCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResult<int>> HandleAsync(UpdateNewsCategoryCommand request, CancellationToken cancellationToken = default)
        {
            var newsCategory = await _dbContext
               .Set<NewsCategory>()
               .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            newsCategory.Update(
                name: request.Name,
                description: request.Description
               );

            await _dbContext.SaveChangesAsync(cancellationToken);
            return ApiResult.From(newsCategory.Id);
        }
    }
}
