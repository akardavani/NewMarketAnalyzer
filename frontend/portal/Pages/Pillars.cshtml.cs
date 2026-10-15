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
  public class PillarsModel : PageModel
  {
    private readonly FundApiClient _client;

    public IReadOnlyCollection<PillarRolesResumeFileResult> PillarRolesResumeFileResults { get; private set; }

    public PillarsModel(FundApiClient client)
    {
      _client = client;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellation)
    {
      var response = await _client.GetAllPillarRolesResumeFile(cancellation);
      if (response.Successed)
      {
        PillarRolesResumeFileResults = response.Data;
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

    public async Task<IActionResult> OnGetDownload(string AttachmentId, CancellationToken cancellation)
    {
      return await _client.LoadFile(AttachmentId, cancellation);
    }
  }
}
