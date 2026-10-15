using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MarketAnalyzer.ApiClient;
using MarketAnalyzer.ApiClient.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FormFile = MarketAnalyzer.ApiClient.Models.FormFile;

namespace MarketAnalyzer.Portal.Pages
{
  public class IssuanceRequestModel : PageModel
  {
    private readonly FundApiClient _client;

    [BindProperty]
    public string CompanyBankAccount { get; set; }

    [BindProperty]
    public int? PaymentMethodId { get; set; }

    [BindProperty]
    public int? UnitTypeId { get; set; }

    [BindProperty]
    public decimal? Amount { get; set; }

    [BindProperty]
    public string DepositSlipNumber { get; set; }

    [BindProperty]
    public DateOnly? DepositDate { get; set; }

    [BindProperty]
    public IFormFile DepositSlipPicture { get; set; }

    [BindProperty]
    public string Description { get; set; }

    [BindProperty]
    public int? GatewayId { get; set; }

    public DateOnly? NavDate { get; private set; }
    public decimal IssuanceNav { get; private set; }
    public decimal CancellationNav { get; private set; }
    public SelectList Gateways { get; private set; }
    public SelectList CompanyBankAccounts { get; private set; }
    public SelectList PaymentMethods { get; private set; }
    public SelectList UnitTypes { get; private set; }

    public IssuanceRequestModel(FundApiClient client)
    {
      _client = client;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellation)
    {
      await InitializeAsync(cancellation);
      return Page();
    }

    public async Task<IActionResult> OnPostDepositSlipAsync(CancellationToken cancellation)
    {
      if (!ModelState.IsValid)
      {
        return await OnErrorAsync(cancellation);
      }

      if (string.IsNullOrEmpty(CompanyBankAccount))
      {
        ModelState.AddModelError(nameof(CompanyBankAccount), "انتخاب شماره حساب صندوق الزامی است.");
      }
      if (string.IsNullOrEmpty(DepositSlipNumber))
      {
        ModelState.AddModelError(nameof(DepositSlipNumber), "وارد کردن شماره فیش واریزی الزامی است.");
      }
      if (DepositDate is null)
      {
        ModelState.AddModelError(nameof(DepositDate), "وارد کردن تاریخ واریز الزامی است.");
      }
      if (DepositSlipPicture is null)
      {
        ModelState.AddModelError(nameof(DepositSlipPicture), "بارگذاری تصویر فیش واریزی الزامی است.");
      }

      if (!ModelState.IsValid)
      {
        return await OnErrorAsync(cancellation);
      }

      var request = new CreatePortalIssuanceRequest()
      {
        Amount = Amount.Value,
        PaymentMethodId = PaymentMethodId.Value,
        UnitTypeId = UnitTypeId.Value,
        CompanyBankAccount = CompanyBankAccount,
        DepositSlipNumber = DepositSlipNumber,
        DepositDate = DepositDate.Value,
        Description = Description,
        DepositSlipPicture =
         new FormFile()
         {
           Size = DepositSlipPicture.Length,
           Name = DepositSlipPicture.FileName,
           Type = DepositSlipPicture.ContentType,
           Content = await ConvertToByteArrayAsync(DepositSlipPicture)
         }
      };

      var result = await _client.CreateIssuanceRequest(request, cancellation);
      if (result.Successed)
      {
        TempData["RequestId"] = result.Data.ToString();
        TempData["RequestType"] = "صدور";
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

    private async Task<string> ConvertToByteArrayAsync(IFormFile depositSlipPicture)
    {
      using (var memoryStream = new MemoryStream())
      {
        await depositSlipPicture.CopyToAsync(memoryStream);
        return Convert.ToBase64String(memoryStream.ToArray());
      }
    }

    public async Task<IActionResult> OnPostPaymentGatewayAsync(CancellationToken cancellation)
    {
      if (!ModelState.IsValid)
      {
        return await OnErrorAsync(cancellation);
      }

      if (GatewayId is null)
      {
        ModelState.AddModelError(nameof(GatewayId), "انتخاب درگاه پرداخت الزامی است.");
      }

      if (!ModelState.IsValid)
      {
        return await OnErrorAsync(cancellation);
      }

      var request = new DepositRequest()
      {
        Amount = Amount.Value,
        GatewayId = GatewayId.Value,
        CallbackUrl = Url.PageLink(pageName: "OnlineDepositCallback", protocol: Request.Scheme, host: Request.Host.ToString())
      };
      var result = await _client.DepositRequestAsync(request, cancellation);
      if (result.Successed)
      {
        TempData[nameof(VerifyOnlineDepositModel.DepositId)] = result.Data.ToString();
        return RedirectToPage("VerifyOnlineDeposit");
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

    private async Task<IActionResult> OnErrorAsync(CancellationToken cancellation)
    {
      await InitializeAsync(cancellation);
      return Page();
    }

    private async Task InitializeAsync(CancellationToken cancellation)
    {
      var response = await _client.GetIssuanceRequestViewDataAsync(cancellation);
      if (response.Successed)
      {
        NavDate = response.Data.NavDate.Value;
        IssuanceNav = response.Data.IssuanceNav;
        CancellationNav = response.Data.CancellationNav;
        Gateways = new SelectList(
          response.Data.Gateways,
          nameof(ClientSelectOption.Value),
          nameof(ClientSelectOption.Text));
        CompanyBankAccounts = new SelectList(
          response.Data.CompanyBankAccounts,
          nameof(ClientSelectOption.Value),
          nameof(ClientSelectOption.Text));
        PaymentMethods = new SelectList(
          response.Data.PaymentMethods,
          nameof(ClientSelectOption.Value),
          nameof(ClientSelectOption.Text));
        UnitTypes = new SelectList(
          response.Data.UnitTypes,
          nameof(ClientSelectOption.Value),
          nameof(ClientSelectOption.Text));
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
