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
  public class SejamKycModel : PageModel
  {

    private readonly FundApiClient _client;

    public SejamKycModel(FundApiClient client)
    {
      _client = client;
    }

    [BindProperty]
    public string NationalCode { get; set; }

    [BindProperty]
    public int OtpLifetime { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "کد ارسال شده را وارد کنید.")]
    public string Otp { get; set; }

    [BindProperty]
    [CaptchaValidator]
    [Required(ErrorMessage = "کد امنیتی الزامی است.")]
    public string CaptchaCode { get; set; }

    public void OnGet()
    {
      if (TempData.ContainsKey("NationalCode"))
      {
        NationalCode = TempData["NationalCode"].ToString();
        OtpLifetime = (int)TempData["OtpLifetime"];
      }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellation)
    {
      if (!ModelState.IsValid)
      {
        CaptchaCode = null;
        this.ResetCaptcha();
        return Page();
      }

      var result = await _client.SejamKycAsync(new SejamKycRequest()
      {
        NationalCode = NationalCode,
        Otp = Otp,
      }, cancellation);

      if (result.Successed)
      {
        TempData.Add("NationalCode", result.Data.NationalCode);
        TempData.Add("ConfirmCode", result.Data.ConfirmCode);
        TempData.Add("SejamProfileHistoryId", result.Data.SejamProfileHistoryId.ToString());
        return RedirectToPage("SejamProfile");
      }
      else
      {
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError("", error);
        }
      }

      CaptchaCode = null;
      this.ResetCaptcha();
      return Page();
    }

    public async Task<IActionResult> OnPostResendSejamKyc(CancellationToken cancellation)
    {
      if (string.IsNullOrEmpty(NationalCode))
      {
        return Redirect("/Signup");
      }

      var result = await _client.ResendSejamKycAsync(NationalCode, cancellation);
      return Content(result, "application/json");
    }
  }
}
