using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Application;
using MarketAnalyzer.Cms.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.cms.Application
{
    internal class NewsQueryHandler : IRequestHandler<NewsQuery, NewsQueruResult>
    {
        private readonly IDbContext _dbContext;
        public NewsQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<NewsQueruResult> HandleAsync(NewsQuery request, CancellationToken cancellationToken = default)
        {
            return _dbContext
                .Set<News>()
                .Select(e => new NewsQueruResult()
                {
                    Id = e.Id,
                    Title = e.Title,
                    Source = e.Source,
                    Summary = e.Summary,
                    IsActive = e.IsActive,
                    Text = e.Text
                })
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        }
    }
}
