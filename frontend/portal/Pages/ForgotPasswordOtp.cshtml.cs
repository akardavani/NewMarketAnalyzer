using Microsoft.AspNetCore.Authorization;
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
  public class ForgotPasswordOtpModel : PageModel
  {
    private readonly IdentityApiClient _identityApiClient;
    private readonly FundApiClient _client;

    public ForgotPasswordOtpModel(IdentityApiClient IdentityApiClient, FundApiClient client)
    {
      _identityApiClient = IdentityApiClient;
      _client = client;
    }

    [BindProperty]
    public string UserName { get; set; }

    [BindProperty]
    public int OtpLifetime { get; set; }

    [Required(ErrorMessage = "کد ارسال شده را وارد کنید.")]
    [BindProperty]
    public string OtpCode { get; set; }

    [Required(ErrorMessage = "رمز عبور الزامی است.")]
    [BindProperty]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "رمز عبور را مجدد وارد کنید.")]
    [BindProperty]
    public string ConfirmPassword { get; set; }

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

      if (TempData.ContainsKey("UserName"))
      {
        UserName = TempData["UserName"].ToString();
        OtpLifetime = (int)TempData["ForgotPasswordOtpLifetime"];
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

      var result = await _identityApiClient.ResetPasswordAsync(new ResetPasswordRequest()
      {
        UserName = UserName,
        NewPassword = NewPassword,
        //ConfirmPassword = ConfirmPassword,
        VerificationCode = OtpCode,
        IpAddress = HttpContext.GetIpAddress()
      }, cancellation);

      if (result.Successed)
      {
        if (result.Data)
        {
          TempData.Clear();
          return Redirect("ResetPasswordResult");
        }
        else
        {
          ModelState.AddModelError("", "عملیات بازیابی رمز عبور با خطا مواجه شد.");
        }
      }
      else
      {
        foreach (string error in result.Errors)
        {
          ModelState.AddModelError("", error);
        }
      }

      CaptchaCode = null;
      this.ResetCaptcha();
      return Page();
    }

    public async Task<IActionResult> OnPostResendForgotPasswordOtp(CancellationToken cancellation)
    {
      if (string.IsNullOrEmpty(UserName))
      {
        return Redirect("/ForgotPassword");
      }

      var result = await _client.ResendForgotPasswordOtpAsync(UserName, HttpContext.GetIpAddress(), cancellation);
      return Content(result, "application/json");
    }
  }
}
