using MarketAnalyzer.Abstractions;
using System.Collections.Generic;
using System.Security.Claims;

namespace MarketAnalyzer.Cms.Application
{
    public class UserMenuQuery : IRequest<IReadOnlyCollection<UserMenuQueryResult>>
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public required int UserId { get; init; }
        /// <summary>
        /// هویت کاربر
        /// </summary>
        public required ClaimsPrincipal Principal { get; init; }
    }
}
