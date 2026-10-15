using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class CreateNewsCommandHandler : IRequestHandler<CreateNewsCommand, ApiResult<int>>
    {
        private readonly IDbContext _dbContext;

        public CreateNewsCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResult<int>> HandleAsync(CreateNewsCommand request, CancellationToken cancellationToken = default)
        {
            var exists = await _dbContext
                .Set<News>()
                .AnyAsync(e => e.Title == request.Title, cancellationToken);

            if (exists)
            {
                return ApiResult.FromError<int>($"خبری با این عنوان {request.Title} قبلا ثبت شده است. ");
            }

            var news = News.Create(
                title: request.Title,
                source: request.Source,
                summary: request.Summary,
                text: request.Text,
                publishDate: default, //TODO: Fill
                isActive: request.IsActive);

            _dbContext
                .Set<News>()
                .Add(news);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return ApiResult.From(news.Id);
        }
    }
}
