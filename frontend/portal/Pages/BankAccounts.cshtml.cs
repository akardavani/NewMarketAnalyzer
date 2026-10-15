using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MarketAnalyzer.ApiClient;
using MarketAnalyzer.ApiClient.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Portal.Pages
{
  [AllowAnonymous]
  public class BankAccountsModel : PageModel
  {
    private readonly FundApiClient _client;

    public BankAccountsModel(FundApiClient client)
    {
      _client = client;
    }

    public IReadOnlyCollection<CompanyBankAccountsResult> CompanyBankAccounts { get; private set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellation)
    {
      var response = await _client.GetAllCompanyBankAccountsAsync(cancellation);
      if (response.Successed)
      {
        CompanyBankAccounts = response.Data.Rows;
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
