using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    public class NewsCategoriesQuery : IRequest<QueryResult<NewsCategoriesQueryResult>>
    {
        public required QueryParam QueryParam { get; init; }
    }

}
