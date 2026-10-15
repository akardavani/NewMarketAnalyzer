using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MarketAnalyzer.ApiClient;
using MarketAnalyzer.ApiClient.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Portal.Pages
{
  [AllowAnonymous]
  public class InvestorProfileModel : PageModel
  {
    private readonly FundApiClient _client;

    [BindProperty]
    [Required(ErrorMessage = "وارد کردن کد الزامی است.")]
    public string KycCode { get; set; }

    public InvestorProfileResult InvestorProfile { get; set; }

    public bool DisplayKycCodeInput { get; set; } = false;

    public string NationalCode { get; set; }

    public InvestorProfileModel(FundApiClient client)
    {
      _client = client;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellation)
    {
      await InitializeAsync(cancellation);
      return Page();
    }

    private async Task<IActionResult> OnErrorAsync(CancellationToken cancellation)
    {
      await InitializeAsync(cancellation);
      return Page();
    }

    private async Task InitializeAsync(CancellationToken cancellation)
    {
      var result = await _client.GetInvestorProfileAsync(cancellation);
      if (result.Successed)
      {
        InvestorProfile = result.Data;
      }
      else
      {
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError("", error);
        }
      }
    }

    public async Task<IActionResult> OnPostGetKycCodeAsync(CancellationToken cancellation)
    {
      await InitializeAsync(cancellation);
      var result = await _client.GetSejamKycCodeAsync(new GetSejamKycCodeRequest()
      {
        NationalCode = InvestorProfile.NationalCode
      }, cancellation);

      if (result.Successed)
      {
        if (result.Data.IsSejami)
        {
          DisplayKycCodeInput = true;
          TempData.Add("NationalCode", InvestorProfile.NationalCode);
          return Page();
        }
        else
        {
          var error = $"امکان احراز هویت در سجام [{result.Data.Reason}] مقدور نیست.";
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
      return Page();
    }

    public async Task<IActionResult> OnPostGetInvestorProfileAsync(CancellationToken cancellation)
    {
      var result = await _client.UpdateInvestorProfileAsync(new UpdateInvestorProfileRequest()
      {
        NationalCode = TempData["NationalCode"].ToString(),
        KycCode = KycCode,
      }, cancellation);

      if (result.Successed)
      {
        InvestorProfile = result.Data;
        DisplayKycCodeInput = false;
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
