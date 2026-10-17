using MarketAnalyzer.Abstractions;
using System.Linq;

namespace MarketAnalyzer.Cms.Application
{
    public class NewsListQuery : Pagination, IRequest<QueryResult<NewsListQueryResult>>
    {
        public string Keyword { get; init; }
    }
}
