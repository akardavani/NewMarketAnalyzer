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
  public class SignupModel : PageModel
  {
    private readonly FundApiClient _client;

    public SignupModel(FundApiClient client)
    {
      _client = client;
    }

    [BindProperty]
    [Required(ErrorMessage = "کد ملی الزامی است.")]
    public string NationalCode { get; set; }

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

      var result = await _client.CheckSejamStateAsync(new CheckSejamStateRequest()
      {
        NationalCode = NationalCode,
      }, cancellation);

      if (result.Successed)
      {
        if (result.Data.IsSejami)
        {
          TempData.Add("NationalCode", NationalCode);
          TempData.Add("OtpLifetime", result.Data.SejamKycLifeTime);
          return RedirectToPage("SejamKyc");
        }
        else
        {
          var error = $"{result.Data.Reason} عدم امکان احراز هویت در سجام:";
          ModelState.AddModelError("", error);
        }
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
  }
}
