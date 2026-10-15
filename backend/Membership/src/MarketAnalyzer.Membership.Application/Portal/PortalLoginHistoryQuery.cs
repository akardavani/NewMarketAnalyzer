using MarketAnalyzer.Abstractions;
using System.Linq;

namespace MarketAnalyzer.Membership.Application
{
    public class PortalLoginHistoryQuery : Pagination, IRequest<QueryResult<PortalLoginHistoryQueryResult>>
    {
        public required int UserId { get; init; }
    }
}
