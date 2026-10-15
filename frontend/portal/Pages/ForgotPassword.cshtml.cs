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
  public class ForgotPasswordModel : PageModel
  {
    private readonly IdentityApiClient _client;

    public ForgotPasswordModel(IdentityApiClient client)
    {
      _client = client;
    }

    [Required(ErrorMessage = "نام کاربری یا کد ملی الزامی است.")]
    [BindProperty]
    public string UserName { get; set; }

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

      var result = await _client.ForgotPasswordAsync(new ForgotPasswordRequest()
      {
        UserName = UserName,
        IpAddress = HttpContext.GetIpAddress()
      }, cancellation);

      if (result.Successed)
      {
        TempData.Clear();
        TempData.Add("UserName", UserName);
        TempData.Add("ForgotPasswordOtpLifetime", result.Data);
        TempData.Keep();
        return RedirectToPage("ForgotPasswordOtp");
      }

      foreach (var error in result.Errors)
      {
        ModelState.AddModelError("", error);
      }

      CaptchaCode = null;
      this.ResetCaptcha();
      return Page();
    }
  }
}
