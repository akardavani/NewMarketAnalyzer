using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
  public class LoginModel : PageModel
  {
    private readonly IdentityApiClient _client;

    public LoginModel(IdentityApiClient client)
    {
      _client = client;
    }

    [BindProperty]
    [Required(ErrorMessage = "نام کاربری یا کد ملی الزامی است.")]
    public string UserName { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "رمز عبور الزامی است.")]
    public string Password { get; set; }

    [BindProperty]
    [CaptchaValidator]
    [Required(ErrorMessage = "کد امنیتی الزامی است.")]
    public string CaptchaCode { get; set; }

    public IActionResult OnGet()
    {
      if (User.Identity.IsAuthenticated)
      {
        return Redirect("InvestorProfile");
      }

      return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellation)
    {
      if (!ModelState.IsValid)
      {
        CaptchaCode = null;
        this.ResetCaptcha();
        return Page();
      }

      var result = await _client.LoginAsync(new LoginRequest()
      {
        UserName = UserName,
        Password = Password,
        LoginType = LoginType.StaticPassword,
        UserAgent = Request.Headers.UserAgent,
        IpAddress = HttpContext.GetIpAddress(),
      }, cancellation);

      if (result.Successed)
      {
        if (result.Data.RequiredTfa)
        {
          TempData["UserName"] = UserName;
          TempData["OtpLifetime"] = result.Data.Lifetime;
          TempData["PhoneNumber"] = MaskPhoneNumber(result.Data.PhoneNumber);
          return RedirectToPage("LoginOtp");
        }
        else
        {
          await this.SignInAsync(result.Data);
          return RedirectToPage("Index");
        }
      }

      foreach (var error in result.Errors)
      {
        ModelState.AddModelError("", error);
      }

      CaptchaCode = null;
      this.ResetCaptcha();
      return Page();
    }

    private static string MaskPhoneNumber(string number)
    {
      return $"{number[..4]}*****{number.Substring(9, 2)}";
    }
  }
}
