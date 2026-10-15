using MarketAnalyzer.Abstractions;
using System.Linq;

namespace MarketAnalyzer.Membership.Application
{
    public class RolesQuery : Pagination, IRequest<QueryResult<RolesQueryResult>>
    {
        /// <summary>
        /// نام
        /// </summary>
        public required string Name { get; init; }
        /// <summary>
        /// نام نمایشی
        /// </summary>
        public required string DisplayName { get; init; }
        /// <summary>
        /// فعال/غیرفعال
        /// </summary>
        public required bool? IsActive { get; init; }
    }
}
