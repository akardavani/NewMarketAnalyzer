using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Api.Models;
using MarketAnalyzer.Infrastructure.Authentication.ApiKey;
using MarketAnalyzer.Membership.Application;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Api.Controllers
{
    [RouteInfo(DisplayName = "حساب کاربری")]
    public class AccountController : FundControllerBase
    {
        [HttpPost]
        [InternalApi]
        [RouteIgnore]
        [ProducesResponseType(typeof(ApiResult<AuthResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellation)
        {
            var command = new LoginCommand()
            {
                UserName = request.UserName,
                Password = request.Password,
                UserAgent = request.UserAgent,
                LoginType = request.LoginType,
                ApplicationId = ApplicationId,
                IpAddress = string.IsNullOrEmpty(request.IpAddress) ? HttpContext.GetIpAddress() : request.IpAddress
            };
            var result = await Mediator.ExecuteAsync(command, cancellation);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [InternalApi]
        [RouteIgnore]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request, CancellationToken cancellation)
        {
            var command = new ForgotPasswordCommand()
            {
                UserName = request.UserName,
                IpAddress = request.IpAddress,
                ApplicationId = ApplicationId
            };
            var result = await Mediator.ExecuteAsync(command, cancellation);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [InternalApi]
        [RouteIgnore]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request, CancellationToken cancellation)
        {
            var command = new ResetPasswordCommand()
            {
                UserName = request.UserName,
                NewPassword = request.NewPassword,
                VerificationCode = request.VerificationCode,
                ApplicationId = ApplicationId,
                IpAddress = request.IpAddress,
            };
            var result = await Mediator.ExecuteAsync(command, cancellation);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [RouteIgnore]
        [RouteInfo(DisplayName = "بازنشانی رمز عبور")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ForceResetPassword(ForceResetPasswordRequest request, CancellationToken cancellation)
        {
            var command = new ForceResetPassword()
            {
                UserId = request.Id,
                Password = request.Password,
                IpAddress = HttpContext.GetIpAddress()
            };
            var result = await Mediator.ExecuteAsync(command, cancellation);
            return Ok(ApiResult.From(result));
        }

        [HttpGet]
        [RouteIgnore]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChangePasswordOtpCode(CancellationToken cancellation)
        {
            var query = new ChangePasswordOtpCodeQuery()
            {
                UserId = CurrentUserId,
                IpAddress = HttpContext.GetIpAddress()
            };
            var result = await Mediator.ExecuteAsync(query, cancellation);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [RouteIgnore]
        [RouteInfo(DisplayName = "تغییر رمز عبور")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellation)
        {
            var command = new ChangePasswordCommand()
            {
                UserId = CurrentUserId,
                OldPassword = request.OldPassword,
                NewPassword = request.NewPassword,
                VerificationCode = request?.VerificationCode,
                ApplicationId = ApplicationId,
            };
            var result = await Mediator.ExecuteAsync(command, cancellation);
            return Ok(ApiResult.From(result));
        }

        [HttpGet]
        [RouteInfo(DisplayName = "گزارش ورود و خروج کاربران")]
        [ProducesResponseType(typeof(ApiResult<QueryResult<UsersLoginHistoryQueryResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UsersLoginHistory([FromQuery] UsersLoginHistoryQueryRequest request, CancellationToken cancellationToken)
        {
            var query = new UsersLoginHistoryQuery()
            {
                UserName = request.UserName,
                LoginType = request.LoginType,
                UserAgent = request.UserAgent,
                OrderBy = request.OrderBy,
                Page = request.Page,
                IpAddress = request.IpAddress,
                ApplicationId = request.ApplicationId,
                ResultId = request.ResultId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Rows = request.Rows
            };

            var result = await Mediator.ExecuteAsync(query, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpGet]
        [RouteIgnore]
        [ProducesResponseType(typeof(ApiResult<QueryResult<LoginHistoryQueryResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UserLoginHistory([FromQuery] LoginHistoryRequest request, CancellationToken cancellationToken)
        {
            var query = new LoginHistoryQuery()
            {
                UserId = CurrentUserId,
                LoginType = request.LoginType,
                UserAgent = request.UserAgent,
                OrderBy = request.OrderBy,
                Page = request.Page,
                IpAddress = request.IpAddress,
                ApplicationId = request.ApplicationId,
                ResultId = request.ResultId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Rows = request.Rows
            };

            var result = await Mediator.ExecuteAsync(query, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpGet]
        [RouteInfo(DisplayName = "گزارش ورود و خروج ناشناس")]
        [ProducesResponseType(typeof(ApiResult<QueryResult<UnknownLoginHistoryQueryResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UnknownLoginHistory([FromQuery] UnknownLoginHistoryQueryRequest request, CancellationToken cancellationToken)
        {
            var query = new UnknownLoginHistoryQuery()
            {
                UserName = request.UserName,
                LoginType = request.LoginType,
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent,
                ApplicationId = request.ApplicationId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Rows = request.Rows
            };

            var result = await Mediator.ExecuteAsync(query, cancellationToken);
            return Ok(ApiResult.From(result));
        }
    }
}
