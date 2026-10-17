using MarketAnalyzer.Abstractions;
using System;
using System.Linq;

namespace MarketAnalyzer.Cms.Application
{
    public class ProspectusesQuery : Pagination, IRequest<QueryResult<ProspectusesQueryResult>>
    {
        public required DateOnly? FromDate { get; init; }
        public required DateOnly? ToDate { get; init; }
    }
}
