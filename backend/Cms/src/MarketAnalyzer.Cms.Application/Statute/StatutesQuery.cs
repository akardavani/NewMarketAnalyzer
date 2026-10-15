using MarketAnalyzer.Abstractions;
using System;
using System.Linq;

namespace MarketAnalyzer.Cms.Application
{
    public class StatutesQuery : Pagination, IRequest<QueryResult<StatutesQueryResult>>
    {
        public required DateOnly? FromDate { get; init; }
        public required DateOnly? ToDate { get; init; }
    }
}
