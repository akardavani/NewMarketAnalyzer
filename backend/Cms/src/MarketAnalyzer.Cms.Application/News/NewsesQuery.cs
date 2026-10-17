using MarketAnalyzer.Abstractions;
using System.Linq;

namespace MarketAnalyzer.Cms.Application
{
    public class NewsesQuery : IRequest<QueryResult<NewsesQueruResult>>
    {
        public required QueryParam QueryParam { get; init; }
    }
   
}
