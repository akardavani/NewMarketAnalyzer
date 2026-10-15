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
  public class InvestorCardexModel : PageModel
  {
    private readonly FundApiClient _client;

    [BindProperty(SupportsGet = true)]
    public DateOnly? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly? ToDate { get; set; }

    public InvestorCardexResult InvestorCardexResult;

    private List<RequestCardexesQueryItemsResult> _cardexRows;
    public List<RequestCardexesQueryItemsResult> CardexRows => _cardexRows ??= [];

    public InvestorCardexModel(FundApiClient client)
    {
      _client = client;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
      var response = await _client.GetInvestorCardexAsync(FromDate, ToDate, cancellationToken);
      if (response.Successed)
      {
        InvestorCardexResult = response.Data;
        _cardexRows = InvestorCardexResult.Items;
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

    public async Task<IActionResult> OnPostExportReportAsync(CancellationToken cancellation)
    {
      var response = await _client.ExportInvestorCardexAsync(FromDate, ToDate, cancellation);

      if (response.Successed)
      {
        var file = new FileContentResult(response.Data.Data, response.Data.ContentType)
        {
          FileDownloadName = response.Data.FileName,
        };
        if (file is not null)
        {
          return file;
        }
        return NotFound();
      }

      foreach (var error in response.Errors)
      {
        ModelState.AddModelError("", error);
      }

      return Page();
    }
  }
}
