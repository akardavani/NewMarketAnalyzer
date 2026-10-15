using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MarketAnalyzer.ApiClient;
using MarketAnalyzer.ApiClient.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Portal.Pages
{
  [AllowAnonymous]
  public class SuperUnitHoldersModel : PageModel
  {
    private readonly FundApiClient _client;

    public SuperUnitHoldersModel(FundApiClient client)
    {
      _client = client;
    }

    private ICollection<SuperUnitsResult> _rows;
    public ICollection<SuperUnitsResult> Rows => _rows ??= [];

    public async Task OnGetAsync(CancellationToken cancellation)
    {
      var response = await _client.GetAllSuperUnitsAsync(cancellation);
      if (response.Successed)
      {
        _rows = response.Data.Rows;
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
