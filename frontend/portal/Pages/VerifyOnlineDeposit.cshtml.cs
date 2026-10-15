using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MarketAnalyzer.ApiClient;
using MarketAnalyzer.ApiClient.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Portal.Pages
{
  public class VerifyOnlineDepositModel : PageModel
  {
    private readonly FundApiClient _client;

    public VerifyOnlineDepositModel(FundApiClient client)
    {
      _client = client;
    }

    [BindProperty]
    [Required(ErrorMessage = "شناسه درخواست الزاامی است.")]
    public long? DepositId { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "کد ارسال شده را وارد کنید.")]
    public string VerificationCode { get; set; }

    public IActionResult OnGet()
    {
      ReadTempData();
      if (DepositId is null)
      {
        TempData.Clear();
        return Redirect("IssuanceRequest");
      }
      else
      {
        return Page();
      }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellation)
    {
      if (!ModelState.IsValid)
      {
        return Page();
      }

      var request = new VerifyOnlineDepositRequest()
      {
        DepositId = DepositId.Value,
        VerificationCode = VerificationCode
      };
      var result = await _client.VerifyOnlineDepositAsync(request, cancellation);
      if (result.Successed)
      {
        return Content(result.Data, "text/html", Encoding.UTF8);
      }
      else
      {
        return Page();
      }
    }

    private void ReadTempData()
    {
      if (TempData.TryGetValue(nameof(DepositId), out var value) &&
          long.TryParse(value?.ToString(), out var depositId))
      {
        DepositId = depositId;
      }
    }
  }
}
