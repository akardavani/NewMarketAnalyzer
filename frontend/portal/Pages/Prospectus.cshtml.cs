using Microsoft.AspNetCore.Authorization;
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
  [AllowAnonymous]
  public class ProspectusModel : PageModel
  {
    private readonly FundApiClient _client;

    public ProspectusModel(FundApiClient client)
    {
      _client = client;
    }

    public ProspectusesResult InitialProspectus { get; private set; }
    public IEnumerable<ProspectusesResult> Prospectuses { get; private set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellation)
    {
      await InitializeAsync(cancellation);
      return Page();
    }

    public async Task<IActionResult> OnGetDownloadAsync(string attachmentId, CancellationToken cancellation)
    {
      await InitializeAsync(cancellation);
      var fileContent = await _client.LoadFile(attachmentId, cancellation);
      if (fileContent is not null)
      {
        return fileContent;
      }
      return NotFound();
    }

    public async Task InitializeAsync(CancellationToken cancellation)
    {
      var response = await _client.GetAllProspectusesAsync(cancellation);
      if (response.Successed)
      {
        Prospectuses = response.Data.Rows;
        InitialProspectus = Prospectuses.OrderBy(e => e.Date).FirstOrDefault();
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
