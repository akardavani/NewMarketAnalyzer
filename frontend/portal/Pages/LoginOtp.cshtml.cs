using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MarketAnalyzer.ApiClient;
using MarketAnalyzer.Captcha;
using MarketAnalyzer.IdentityClient;
using MarketAnalyzer.IdentityClient.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Portal.Pages
{
  [AllowAnonymous]
  public class LoginOtpModel : PageModel
  {
    private readonly FundApiClient _client;
    private readonly IdentityApiClient _identityApiClient;

    [BindProperty]
    public string UserName { get; set; }

    [BindProperty]
    public string PhoneNumber { get; set; }

    [BindProperty]
    public int OtpLifetime { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "رمز عبور الزامی است.")]
    public string Password { get; set; }

    [BindProperty]
    [CaptchaValidator]
    [Required(ErrorMessage = "کد امنیتی الزامی است.")]
    public string CaptchaCode { get; set; }

    public LoginOtpModel(FundApiClient client, IdentityApiClient identityApiClient)
    {
      _client = client;
      _identityApiClient = identityApiClient;
    }

    public IActionResult OnGet()
    {
      if (User.Identity.IsAuthenticated)
      {
        return Redirect("InvestorProfile");
      }

      ReadTempData();

      if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(PhoneNumber))
      {
        return Redirect("Login");
      }
      else
      {
        return Page();
      }
    }

    public async Task<IActionResult> OnPostLoginAsync(CancellationToken cancellation)
    {
      if (string.IsNullOrEmpty(UserName))
      {
        return Redirect("/Login");
      }

      if (!ModelState.IsValid)
      {
        CaptchaCode = null;
        this.ResetCaptcha();
        return Page();
      }

      var result = await _identityApiClient.LoginAsync(new LoginRequest()
      {
        UserName = UserName,
        Password = Password,
        LoginType = LoginType.OneTimePassword,
        UserAgent = Request.Headers.UserAgent,
        IpAddress = HttpContext.GetIpAddress(),
      }, cancellation);

      if (result.Successed)
      {
        await this.SignInAsync(result.Data);
        TempData.Clear();
        return Redirect("InvestorProfile");
      }
      else
      {
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError("", error);
        }
        return Page();
      }
    }

    public async Task<IActionResult> OnPostResendOtpAsync(CancellationToken cancellation)
    {
      if (string.IsNullOrEmpty(UserName))
      {
        return Redirect("/Login");
      }

      var result = await _client.ResendOtpAsync(UserName, cancellation);
      return Content(result, "application/json");
    }

    private void ReadTempData()
    {
      UserName = TempData["UserName"]?.ToString();
      PhoneNumber = TempData["PhoneNumber"]?.ToString();
      OtpLifetime = (int)TempData["OtpLifetime"];
    }
  }
}
