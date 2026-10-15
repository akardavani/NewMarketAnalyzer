using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class NewsesQueryHandler : IRequestHandler<NewsesQuery, QueryResult<NewsesQueruResult>>
    {
        private readonly IDbContext _dbContext;
        public NewsesQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<QueryResult<NewsesQueruResult>> HandleAsync(NewsesQuery request, CancellationToken cancellationToken = default)
        {
            return _dbContext
                     .Set<News>()
                     .QueryAsync(request.QueryParam, e => new NewsesQueruResult
                     {
                         Id = e.Id,
                         Title = e.Title,
                         Source = e.Source,
                         Summary = e.Summary,
                         IsActive = e.IsActive,
                         Text = e.Text
                     }, cancellationToken);
        }
    }
}
