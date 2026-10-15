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
  public class InvestmentPolicyStatementsModel : PageModel
  {
    private readonly FundApiClient _client;

    public InvestmentPolicyStatementsModel(FundApiClient client)
    {
      _client = client;
    }

    private ICollection<InvestmentPolicyStatementsResult> _rows;
    public ICollection<InvestmentPolicyStatementsResult> Rows => _rows ??= [];

    public async Task OnGetAsync(CancellationToken cancellation)
    {
      var response = await _client.GetAllInvestmentPolicyStatementsAsync(cancellation);
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

    public async void OnGetDownload(string attachmentId, CancellationToken cancellation)
    {
      await _client.LoadFile(attachmentId, cancellation);
    }
  }
}
