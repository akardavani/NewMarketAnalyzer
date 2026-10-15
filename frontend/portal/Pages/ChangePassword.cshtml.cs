using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MarketAnalyzer.ApiClient;
using MarketAnalyzer.ApiClient.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Portal.Pages
{
  public class ChangePasswordModel : PageModel
  {
    private readonly FundApiClient _client;

    [BindProperty]
    [Required(ErrorMessage = "رمز عبور فعلی را وارد کنید.")]
    public string CurrentPassword { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "رمز عبور جدید را وارد کنید.")]
    public string NewPassword { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "رمز عبور جدید را مجدد وارد کنید.")]
    public string RepeatPassword { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "کد ارسال شده را وارد کنید.")]
    public string VerificationCode { get; set; }

    public ChangePasswordModel(FundApiClient client)
    {
      _client = client;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostGetOtpCodeAsync(CancellationToken cancellation)
    {
      var result = await _client.GetChangePasswordOtpCodeAsync(HttpContext.GetIpAddress(), cancellation);

      if (result.Successed)
      {
        return Page();
      }
      else
      {
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError("", error);
        }
      }
      return Page();
    }

    public async Task<IActionResult> OnPostChangePasswordAsync(CancellationToken cancellation)
    {
      if (!ModelState.IsValid)
      {
        return Page();
      }

      var result = await _client.ChangePasswordAsync(new ChangePasswordRequest()
      {

      }, cancellation);

      if (result.Successed)
      {
        return Page();
      }
      else
      {
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError("", error);
        }
      }
      return Page();
    }
  }
}
