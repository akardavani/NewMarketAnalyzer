using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    public class UpdateNewsCommandHandler : IRequestHandler<UpdateNewsCommand, ApiResult<int>>
    {
        private readonly IDbContext _dbContext;

        public UpdateNewsCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResult<int>> HandleAsync(UpdateNewsCommand request, CancellationToken cancellationToken = default)
        {
            var news = await _dbContext
               .Set<News>()
               .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            news.Update(
              title: request.Title,
              source: request.Source,
              summary: request.Summary,
              text: request.Text
               );

            await _dbContext.SaveChangesAsync(cancellationToken);
            return ApiResult.From(news.Id);
        }
    }
}
