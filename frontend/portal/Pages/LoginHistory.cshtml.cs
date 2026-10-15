using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MarketAnalyzer.ApiClient;
using MarketAnalyzer.ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Portal.Pages
{
  public class LoginHistoryModel : PageModel
  {
    private readonly FundApiClient _client;

    public const int PageSize = 20;

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;
    public int TotalPages { get; private set; }

    private ICollection<LoginHistoryResult> _rows;
    public ICollection<LoginHistoryResult> Rows => _rows ??= [];

    public LoginHistoryModel(FundApiClient client)
    {
      _client = client;
    }

    public async Task OnGetAsync(CancellationToken cancellation)
    {
      var response = await _client.GetLoginHistoryAsync(Math.Max(1, PageNumber), PageSize, cancellation);
      if (response.Successed)
      {
        _rows = response.Data.Rows;
        TotalPages = (int)Math.Ceiling((double)response.Data.ItemsCount / PageSize);
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
