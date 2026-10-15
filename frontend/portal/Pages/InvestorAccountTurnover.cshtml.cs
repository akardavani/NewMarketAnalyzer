using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MarketAnalyzer.ApiClient;
using MarketAnalyzer.ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Portal.Pages
{
  public class InvestorAccountTurnoverModel : PageModel
  {
    private readonly FundApiClient _client;

    [BindProperty(SupportsGet = true)]
    public DateOnly? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly? ToDate { get; set; }

    public decimal TotalDebit { get; set; }

    public decimal TotalCredit { get; set; }

    public decimal TotalBalance { get; set; }

    private IReadOnlyCollection<InvestorAccountTurnoverResult.Item> _rows;
    public IReadOnlyCollection<InvestorAccountTurnoverResult.Item> Rows => _rows ??= [];

    public InvestorAccountTurnoverModel(FundApiClient client)
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
      var response = await _client.GetInvestorAccountTurnoverAsync(FromDate, ToDate, cancellation);
      if (response.Successed)
      {
        _rows = response.Data.Items;
        TotalDebit = response.Data.TotalDebit;
        TotalCredit = response.Data.TotalCredit;
        TotalBalance = response.Data.TotalBalance;
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
