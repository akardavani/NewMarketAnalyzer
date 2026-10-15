using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Api.Models;
using MarketAnalyzer.Membership.Application;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Api.Controllers
{
    [RouteInfo(DisplayName = "نقش‌ها")]
    public class RoleController : FundControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<QueryResult<RolesQueryResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] RolesQueryRequest request, CancellationToken cancellationToken)
        {
            var query = new RolesQuery()
            {
                DisplayName = request.DisplayName,
                IsActive = request.IsActive,
                Name = request.Name,
                OrderBy = request.OrderBy,
                Page = request.Page,
                Rows = request.Rows
            };

            var result = await Mediator.ExecuteAsync(query, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<RoleQueryResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var query = new RoleQuery()
            {
                Id = id
            };

            var result = await Mediator.ExecuteAsync(query, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CreateRoleRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateRoleCommand()
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
                IsActive = request.IsActive,
            };

            var result = await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(UpdateRoleRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateRoleCommand()
            {
                Id = request.Id,
                Name = request.Name,
                DisplayName = request.DisplayName,
                IsActive = request.IsActive
            };

            var result = await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Remove(RemoveRoleRequest request, CancellationToken cancellationToken)
        {
            var command = new RemoveRoleCommand()
            {
                Id = request.Id,
                OperatorId = CurrentUserId,
                RowVersion = request.RowVersion
            };

            var result = await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpGet]
        [RouteInfo(DisplayName = "مشاهده دسترسی‌ها")]
        [ProducesResponseType(typeof(ApiResult<List<RolePermissionsQueryResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Permissions(int roleId, CancellationToken cancellationToken)
        {
            var query = new RolePermissionsQuery()
            {
                RoleId = roleId
            };

            var result = await Mediator.ExecuteAsync(query, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [RouteInfo(DisplayName = "ویرایش دسترسی‌ها")]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePermissions(UpdateRolePermissionsRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateRolePermissionsCommand()
            {
                RoleId = request.RoleId,
                OperatorId = CurrentUserId,
                Routes = request.Routes
            };

            await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(ApiResult.Success);
        }


        [HttpGet]
        [RouteIgnore]
        [ProducesResponseType(typeof(ApiResult<List<SelectOption>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> QueryOptions(CancellationToken cancellationToken)
        {
            var query = new RolesSelectOptionQuery();
            var result = await Mediator.ExecuteAsync(query, cancellationToken);
            return Ok(ApiResult.From(result));
        }
    }
}
