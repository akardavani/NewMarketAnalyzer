using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class NewsListQueryHandler : IRequestHandler<NewsListQuery, QueryResult<NewsListQueryResult>>
    {
        private readonly IDbContext _dbContext;

        public NewsListQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<NewsListQueryResult>> HandleAsync(NewsListQuery request, CancellationToken cancellationToken = default)
        {
            var query = _dbContext
                .Set<News>()
                .Where(e => e.IsActive && (string.IsNullOrEmpty(request.Keyword) || e.Title.Contains(request.Keyword)));

            var rows = await query
                .Select(e => new NewsListQueryResult()
                {
                    Id = e.Id,
                    Title = e.Title,
                    Date = e.PublishDate
                })
                .Pagination(request)
                .ToListAsync(cancellationToken);

            return new QueryResult<NewsListQueryResult>()
            {
                Rows = rows,
                ItemsCount = await query.LongCountAsync(cancellationToken)
            };
        }
    }
}
