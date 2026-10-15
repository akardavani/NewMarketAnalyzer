using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MarketAnalyzer.ApiClient.Models;
using MarketAnalyzer.ApiClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace MarketAnalyzer.Portal.Pages
{
  [AllowAnonymous]
  public class StatuteModel : PageModel
  {
    private readonly FundApiClient _client;

    public StatuteModel(FundApiClient client)
    {
      _client = client;
    }

    public StatutesResult InitialStatute { get; private set; }
    public IEnumerable<StatutesResult> Statutes { get; private set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellation)
    {
      var response = await _client.GetAllStatusesAsync(cancellation);
      if (response.Successed)
      {
        Statutes = response.Data.Rows;
        InitialStatute = Statutes.OrderBy(e => e.Date).FirstOrDefault();
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

    public async Task<IActionResult> OnGetDownload(string attachmentId, CancellationToken cancellation)
    {
      return await _client.LoadFile(attachmentId, cancellation);
    }
  }
}
