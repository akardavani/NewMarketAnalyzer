using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Api.Models;
using MarketAnalyzer.Membership.Application;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Api.Controllers
{
    [RouteInfo(DisplayName = "کاربران")]
    public class UserController : FundControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<QueryResult<UsersQueryResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] UserQueryRequest request, CancellationToken cancellationToken)
        {
            var query = new UsersQuery()
            {
                FullName = request.FullName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                TwoFactorAuthentication = request.TwoFactorAuthentication,
                IsActive = request.IsActive
            };

            var result = await Mediator.ExecuteAsync(query, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<UserQueryResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var query = new UserQuery()
            {
                Id = id
            };

            var result = await Mediator.ExecuteAsync(query, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateUserCommand()
            {
                FullName = request.FullName,
                UserName = request.UserName,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                EmailConfirmed = false,
                TwoFactorAuthentication = request.TwoFactorAuthentication,
                Roles = request.Roles,
                ApplicationId = Membership.Domain.Application.AdminWeb.Id,
                IpAddresses = request.IpAddressesList,
                Claims = null
            };

            var result = await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateUserCommand()
            {
                Id = request.Id,
                FullName = request.FullName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                TwoFactorAuthentication = request.TwoFactorAuthentication,
                Email = request.Email,
                Roles = request.Roles,
                ApplicationId = ApplicationId,
                IpAddresses = request.IpAddressesList
            };

            var result = await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Remove(RemoveUserRequest request, CancellationToken cancellationToken)
        {
            var command = new RemoveUserCommand()
            {
                Id = request.Id
            };

            var result = await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [RouteInfo(DisplayName = "بازنشانی رمز عبور")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ForceResetPassword(AdminForceResetPasswordRequest request, CancellationToken cancellation)
        {
            var command = new AdminForceResetPasswordCommand()
            {
                UserId = request.UserId
            };
            var result = await Mediator.ExecuteAsync(command, cancellation);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [RouteInfo(DisplayName = "فعالسازی/غیرفعالسازی")]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Activation(ActiveDeactiveUserRequest request, CancellationToken cancellationToken)
        {
            var command = new UserActivationCommand()
            {
                UserId = request.UserId
            };

            var result = await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(ApiResult.From(result));
        }
    }
}
