using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System.Threading;
using MarketAnalyzer.ApiClient;
using System;
using System.ComponentModel.DataAnnotations;
using MarketAnalyzer.ApiClient.Models;

namespace MarketAnalyzer.Portal.Pages
{
  public class CancellationRequestModel : PageModel
  {
    private readonly FundApiClient _client;

    /// <summary>
    /// تعداد واحد جهت ابطال
    /// </summary>
    [BindProperty]
    [Required(ErrorMessage = "وارد کردن تعداد واحد جهت ابطال الزامی است.")]
    public long? Quantity { get; set; }

    /// <summary>
    /// تاریخ درخواست
    /// </summary>
    [BindProperty]
    public DateTime RequestDate { get; set; }

    /// <summary>
    /// تعداد واحد قابل ابطال
    /// </summary>
    [BindProperty]
    public long CancelableUnits { get; set; }

    /// <summary>
    /// تعداد واحد وثیقه شده
    /// </summary>
    [BindProperty]
    public long CollateralizedUnits { get; set; }

    /// <summary>
    /// تعداد واحد در انتظار تایید
    /// </summary>
    [BindProperty]
    public long PendingCancellationRequests { get; set; }

    public DateOnly NavDate { get; private set; }
    public decimal IssuanceNav { get; private set; }
    public decimal CancellationNav { get; private set; }

    public CancellationRequestModel(FundApiClient client)
    {
      _client = client;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellation)
    {
      await InitializeAsync(cancellation);
      return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellation)
    {
      if (!ModelState.IsValid)
      {
        return await OnErrorAsync(cancellation);
      }
      else
      {
        var request = new CreatePortalCancellationRequest()
        {
          Quantity = Quantity.Value
        };

        var result = await _client.CreateCancellationRequest(request, cancellation);
        if (result.Successed)
        {
          TempData.Add("RequestId", result.Data.ToString());
          TempData.Add("RequestType", "ابطال");
          return RedirectToPage("RequestList");
        }
        else
        {
          foreach (var error in result.Errors)
          {
            ModelState.AddModelError("", error);
          }
          return await OnErrorAsync(cancellation);
        }
      }
    }

    private async Task<IActionResult> OnErrorAsync(CancellationToken cancellation)
    {
      await InitializeAsync(cancellation);
      return Page();
    }

    private async Task InitializeAsync(CancellationToken cancellation)
    {
      var response = await _client.GetCancellationRequestViewDataAsync(cancellation);
      if (response.Successed)
      {
        NavDate = response.Data.NavDate;
        IssuanceNav = response.Data.IssuanceNav;
        CancellationNav = response.Data.CancellationNav;
        RequestDate = DateTime.Now;
        CancelableUnits = response.Data.CancelableUnits;
        CollateralizedUnits = response.Data.CollateralizedUnits;
        PendingCancellationRequests = response.Data.PendingCancellationRequests;
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
