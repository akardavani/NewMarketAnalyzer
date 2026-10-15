using MarketAnalyzer.Abstractions;
using System.Linq;

namespace MarketAnalyzer.Membership.Application
{
    public class UsersQuery : Pagination, IRequest<QueryResult<UsersQueryResult>>
    {
        /// <summary>
        /// نام و نام خانوادگی  
        /// </summary>
        public required string FullName { get; init; }
        /// <summary>
        /// نام کاربری
        /// </summary>
        public required string UserName { get; init; }
        /// <summary>
        /// شماره موبایل
        /// </summary>
        public required string PhoneNumber { get; init; }
        /// <summary>
        /// ایمیل
        /// </summary>
        public required string Email { get; init; }
        /// <summary>
        /// ورود دو عاملی
        /// </summary>
        public required bool? TwoFactorAuthentication { get; init; }
        /// <summary>
        /// فعال
        /// </summary>
        public required bool? IsActive { get; init; }
    }
}
