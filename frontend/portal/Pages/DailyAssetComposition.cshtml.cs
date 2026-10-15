using Microsoft.AspNetCore.Authorization;
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
  [AllowAnonymous]
  public class DailyAssetCompositionModel : PageModel
  {
    private readonly FundApiClient _client;

    public const int PageSize = 15;

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;
    public int TotalPages { get; private set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly? ToDate { get; set; }

    private ICollection<DailyAssetCompositionResult> _rows;
    public ICollection<DailyAssetCompositionResult> Rows => _rows ??= [];

    public DailyAssetCompositionModel(FundApiClient client)
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
      var response = await _client.GetDailyAssetComposition(Math.Max(1, PageNumber), PageSize, FromDate, ToDate, cancellation);
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
