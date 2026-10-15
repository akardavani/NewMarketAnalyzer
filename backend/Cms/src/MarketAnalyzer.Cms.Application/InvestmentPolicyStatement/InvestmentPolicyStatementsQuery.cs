using MarketAnalyzer.Abstractions;
using System;
using System.Linq;

namespace MarketAnalyzer.Cms.Application
{
    public class InvestmentPolicyStatementsQuery : Pagination, IRequest<QueryResult<InvestmentPolicyStatementsQueryResult>>
    {
        public required DateOnly? FromDate { get; init; }
        public required DateOnly? ToDate { get; init; }
        /// <summary>
        /// فعال/غیرفعال
        /// </summary>
        public required bool? IsActive { get; init; }
    }
}
