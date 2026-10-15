using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.IdentityClient;
using MarketAnalyzer.IdentityClient.Models;
using MarketAnalyzer.Ui.Server.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Ui.Server.Controllers;

public class AccountController : Controller
{
    private readonly IdentityApiClient _identityClient;

    public AccountController(IdentityApiClient identityClient)
    {
        _identityClient = identityClient;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] Login model, CancellationToken cancellation)
    {
        if (!ModelState.IsValid)
        {
            return Ok(new IdentityResult<AuthResult>()
            {
                Successed = false,
                Errors = ["کد امنیتی معتبر نمیباشد."]
            });
        }

        var result = await _identityClient.LoginAsync(new LoginRequest()
        {
            UserName = model.UserName,
            Password = model.Password,
            LoginType = model.LoginType,
            UserAgent = Request.Headers.UserAgent,
            IpAddress = HttpContext.GetIpAddress()
        }, cancellation);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPassword model, CancellationToken cancellation)
    {
        if (!ModelState.IsValid)
        {
            return Ok(new IdentityResult<AuthResult>()
            {
                Successed = false,
                Errors = ["کد امنیتی معتبر نمیباشد."]
            });
        }

        var result = await _identityClient.ForgotPasswordAsync(new ForgotPasswordRequest()
        {
            UserName = model.UserName,
            IpAddress = HttpContext.GetIpAddress()
        }, cancellation);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPassword model, CancellationToken cancellation)
    {
        if (!ModelState.IsValid)
        {
            return Ok(new IdentityResult<AuthResult>()
            {
                Successed = false,
                Errors = ["کد امنیتی معتبر نمیباشد."]
            });
        }

        var result = await _identityClient.ResetPasswordAsync(new ResetPasswordRequest()
        {
            UserName = model.UserName,
            NewPassword = model.Password,
            VerificationCode = model.VerificationCode,
            IpAddress = HttpContext.GetIpAddress()
        }, cancellation);

        return Ok(result);
    }
}
