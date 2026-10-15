using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MarketAnalyzer.ApiClient;
using MarketAnalyzer.ApiClient.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Portal.Pages
{
  public class LastInvestmentCertificateModel : PageModel
  {
    private readonly FundApiClient _client;

    public LastInvestmentCertificateModel(FundApiClient client)
    {
      _client = client;
    }

    public LastInvestmentCertificateResult Certificate { get; private set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellation)
    {
      var response = await _client.GetLastInvestmentCertificate(cancellation);
      if (response.Successed)
      {
        if (response.Data is not null)
        {
          Certificate = response.Data;
        }
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

    public async Task<IActionResult> OnPostExportLastInvestmentCertificate(long requestId, int unitTypeId, CancellationToken cancellation)
    {
      var response = await _client.ExportLastInvestmentCertificate(requestId, unitTypeId, cancellation);

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
