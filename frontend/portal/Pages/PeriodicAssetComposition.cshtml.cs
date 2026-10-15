using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MarketAnalyzer.ApiClient;
using MarketAnalyzer.ApiClient.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Portal.Pages
{
  [AllowAnonymous]
  public class PeriodicAssetCompositionModel : PageModel
  {
    private readonly FundApiClient _client;

    [BindProperty(SupportsGet = true)]
    public DateOnly? ToDate { get; set; }

    public PeriodicAssetCompositionResult PeriodicAssetCompositionResult { get; set; }

    public PeriodicAssetCompositionModel(FundApiClient client)
    {
      _client = client;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellation)
    {
      await InitializeAsync(cancellation);
      return Page();
    }

    private async Task InitializeAsync(CancellationToken cancellation)
    {
      var response = await _client.GetPeriodicAssetComposition(ToDate, cancellation);
      if (response.Successed)
      {
        PeriodicAssetCompositionResult = response.Data;
      }
      else
      {
        foreach (var error in response.Errors)
        {
          ModelState.AddModelError("", error);
        }
      }
    }
  }
}
