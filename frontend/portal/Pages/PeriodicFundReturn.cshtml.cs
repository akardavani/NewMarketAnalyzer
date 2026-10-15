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
  public class PeriodicFundReturnModel : PageModel
  {
    private readonly FundApiClient _client;

    [BindProperty(SupportsGet = true)]
    public DateOnly? ToDate { get; set; }

    public PeriodicFundReturnResult PeriodicFundReturns { get; private set; }

    public PeriodicFundReturnModel(FundApiClient client)
    {
      _client = client;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellation)
    {
      var response = await _client.GetPeriodicFundReturnAsync(ToDate, cancellation);
      if (response.Successed)
      {
        PeriodicFundReturns = response.Data;
      }
      else
      {
        foreach (var error in response.Errors)
        {
          ModelState.AddModelError("", error);
        }
      }
      return Page();
    }
  }
}
