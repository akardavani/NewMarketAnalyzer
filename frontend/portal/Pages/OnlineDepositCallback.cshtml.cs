using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MarketAnalyzer.ApiClient;
using MarketAnalyzer.ApiClient.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Portal.Pages
{
  [AllowAnonymous]
  [IgnoreAntiforgeryToken]
  public class OnlineDepositCallbackModel : PageModel
  {
    private readonly FundApiClient _client;

    public OnlineDepositCallbackModel(FundApiClient client)
    {
      _client = client;
    }

    /// <summary>
    /// شناسه داخلی درخواست
    /// </summary>
    [Required]
    [BindProperty]
    public long? DepositId { get; set; }
    /// <summary>
    /// توکن امنیتی بررسی دسترسی ترمینال بانک
    /// </summary>
    [Required]
    [BindProperty]
    public string Checksum { get; set; }

    public bool Successed { get; private set; }
    public string TransactionId { get; private set; }
    public decimal Amount { get; private set; }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellation)
    {
      if (ModelState.IsValid)
      {
        var formData = await Request.ReadFormAsync(cancellation);
        var data = formData.ToDictionary(e => e.Key, e => e.Value);
        var request = new ReceiveOnlineDepositRequest()
        {
          DepositId = DepositId.Value,
          Checksum = Checksum,
          FormData = formData.ToDictionary(e => e.Key, e => e.Value.ToArray())
        };
        var result = await _client.ReceiveOnlineDepositAsync(request, cancellation);
        if (result.Successed)
        {
          await VerifyIssuanceRequestAsync(result.Data, cancellation);
        }
        else
        {
          foreach (var error in result.Errors)
          {
            ModelState.AddModelError("", error);
          }
        }
      }

      return Page();
    }

    private async Task VerifyIssuanceRequestAsync(ReceiveOnlineDepositResult result, CancellationToken cancellation)
    {
      var request = new VerifyOnlineDepositIssuanceRequestRequest()
      {
        DepositId = result.DepositId,
        RequestId = result.RequestId,
        TransactionId = result.TransactionId,
        Amount = result.Amount
      };
      var issuanceResult = await _client.VerifyOnlineDepositIssuanceRequestAsync(request, cancellation);
      if (issuanceResult.Successed)
      {
        Successed = true;
      }
      else
      {
        foreach (var error in issuanceResult.Errors)
        {
          ModelState.AddModelError("", error);
        }
      }
    }
  }
}
