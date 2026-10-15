using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Api.Models;
using MarketAnalyzer.Cms.Application;
using MarketAnalyzer.Membership.Application;
using MarketAnalyzer.SharedKernel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Api.Controllers
{
    public class HomeController : FundControllerBase
    {
        [HttpGet]
        [RouteIgnore]
        [ProducesResponseType(typeof(ApiResult<UserAppData>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAppData(CancellationToken cancellationToken)
        {
            AppState appState;
            var userProfile = await GetUserProfileAsync(cancellationToken);

            appState = AppState.Ready;

            var result = new UserAppData()
            {
                AppState = appState,
                FullName = userProfile.FullName,
                LastLoginDate = userProfile.LastLoginDate,
                Menus = await GetMenusAsync(cancellationToken)
            };

            return Ok(ApiResult.From(result));
        }

        private Task<UserProfileQueryResult> GetUserProfileAsync(CancellationToken cancellation)
        {
            var query = new UserProfileQuery()
            {
                Id = CurrentUserId
            };
            return Mediator.ExecuteAsync(query, cancellation);
        }
        private Task<IReadOnlyCollection<UserMenuQueryResult>> GetMenusAsync(CancellationToken cancellationToken)
        {
            var query = new UserMenuQuery()
            {
                Principal = User,
                UserId = CurrentUserId
            };
            return Mediator.ExecuteAsync(query, cancellationToken);
        }

    }
}
