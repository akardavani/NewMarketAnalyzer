using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MarketAnalyzer.ApiClient;
using MarketAnalyzer.ApiClient.Models;
using MarketAnalyzer.Captcha;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;


namespace MarketAnalyzer.Portal.Pages
{
  [AllowAnonymous]
  public class SejamProfileModel : PageModel
  {
    private readonly FundApiClient _client;
    public SejamProfileResult SejamProfileResult { get; set; }

    [BindProperty]
    public string NationalCode { get; set; }

    [BindProperty]
    public long SejamProfileHistoryId { get; set; }

    [BindProperty]
    public string Otp { get; set; }

    [BindProperty]
    [CaptchaValidator]
    [Required(ErrorMessage = "کد امنیتی الزامی است.")]
    public string CaptchaCode { get; set; }

    public SejamProfileModel(FundApiClient client)
    {
      _client = client;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellation)
    {
      if (TempData.TryGetValue("NationalCode", out var nationalCode) && !string.IsNullOrEmpty(nationalCode?.ToString()))
      {
        NationalCode = TempData["NationalCode"].ToString();
        SejamProfileHistoryId = long.Parse(TempData["SejamProfileHistoryId"].ToString());
        Otp = TempData["ConfirmCode"].ToString();

        await InitializeAsync(cancellation);
        return Page();
      }
      else
      {
        return RedirectToPage("Signup");
      }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellation)
    {
      if (!ModelState.IsValid)
      {
        return await OnErrorAsync(cancellation);
      }

      var result = await _client.Signup(new SignupRequest()
      {
        NationalCode = NationalCode,
        SejamProfileHistoryId = SejamProfileHistoryId,
        Otp = Otp
      }, cancellation);

      if (result.Successed)
      {
        if (result.Data.Value)
        {
          return RedirectToPage("SignupResult");
        }
        else
        {
          ModelState.AddModelError("", "درخواست ثبت نام با خطا مواجه شد.");
        }
      }
      else
      {
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError("", error);
        }
        return await OnErrorAsync(cancellation);
      }
      return Page();
    }

    private async Task<IActionResult> OnErrorAsync(CancellationToken cancellation)
    {
      await InitializeAsync(cancellation);
      CaptchaCode = null;
      this.ResetCaptcha();
      return Page();
    }

    private async Task InitializeAsync(CancellationToken cancellation)
    {
      var result = await _client.GetSejamProfileAsync(new SejamProfileRequest()
      {
        NationalCode = NationalCode,
        SejamProfileHistoryId = SejamProfileHistoryId,
        Otp = Otp
      }, cancellation);

      if (result.Successed)
      {
        SejamProfileResult = result.Data;
      }
      else
      {
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError("", error);
        }
      }
    }
  }
}
