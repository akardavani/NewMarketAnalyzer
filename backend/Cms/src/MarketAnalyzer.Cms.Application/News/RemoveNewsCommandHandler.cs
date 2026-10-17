using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class RemoveNewsCommandHandler : IRequestHandler<RemoveNewsCommand, ApiResult<int>>
    {
        private readonly IDbContext _dbContext;

        public RemoveNewsCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResult<int>> HandleAsync(RemoveNewsCommand request, CancellationToken cancellationToken = default)
        {
            var news = await _dbContext
                .Set<News>()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (news is null)
            {
                return ApiResult.FromError<int>("خبر مورد نظر وجود ندارد.");
            }
            _dbContext.Set<News>().Remove(news);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return ApiResult.From(news.Id);
        }
    }
}
