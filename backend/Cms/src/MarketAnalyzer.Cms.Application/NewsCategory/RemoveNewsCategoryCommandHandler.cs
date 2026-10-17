using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class RemoveNewsCategoryCommandHandler : IRequestHandler<RemoveNewsCategoryCommand, ApiResult<int>>
    {
        private readonly IDbContext _dbContext;

        public RemoveNewsCategoryCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResult<int>> HandleAsync(RemoveNewsCategoryCommand request, CancellationToken cancellationToken = default)
        {
            var newsCategory = await _dbContext
                .Set<NewsCategory>()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (newsCategory is null)
            {
                return ApiResult.FromError<int>("دسته بندی خبر مورد نظر وجود ندارد.");
            }
            _dbContext.Set<NewsCategory>().Remove(newsCategory);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return ApiResult.From(newsCategory.Id);
        }
    }
}
