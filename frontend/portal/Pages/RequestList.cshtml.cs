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
  public class RequestListModel : PageModel
  {
    private readonly FundApiClient _client;

    public const int PageSize = 50;

    [BindProperty(SupportsGet = true)]
    public string RequestResult { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;
    public int TotalPages { get; private set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    private ICollection<RequestsResult> _rows;
    public ICollection<RequestsResult> Rows => _rows ??= [];

    public RequestListModel(FundApiClient client)
    {
      _client = client;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellation)
    {
      if (TempData["RequestId"] is not null)
      {
        RequestResult = $"درخواست {TempData["RequestType"]} با موفقیت انجام شد. شناسه درخواست: {TempData["RequestId"]}";
      }
      await InitializeAsync(cancellation);
      return Page();
    }

    private async Task InitializeAsync(CancellationToken cancellation)
    {
      var response = await _client.GetAllRequestAsync(Math.Max(1, PageNumber), PageSize, FromDate, ToDate, cancellation);
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
