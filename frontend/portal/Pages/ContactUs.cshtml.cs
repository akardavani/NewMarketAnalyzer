using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MarketAnalyzer.ApiClient;
using MarketAnalyzer.ApiClient.Models;
using MarketAnalyzer.Captcha;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Portal.Pages
{
  [AllowAnonymous]
  public class ContactUsModel : PageModel
  {
    private readonly FundApiClient _client;

    public ContactUsModel(FundApiClient client)
    {
      _client = client;
    }

    [BindProperty(SupportsGet = true)]
    public string ContactUsResult { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "نام و نام خانوادگی الزامی است.")]
    public string FullName { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "آدرس ایمیل الزامی است.")]
    public string Email { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "شماره موبایل الزامی است.")]
    public string PhoneNumber { get; set; }

    [BindProperty]
    public string Subject { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "نوشتن پیام الزامی است.")]
    public string Text { get; set; }

    [BindProperty]
    [CaptchaValidator]
    [Required(ErrorMessage = "کد امنیتی الزامی است.")]
    public string CaptchaCode { get; set; }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
      if (!ModelState.IsValid)
      {
        CaptchaCode = null;
        this.ResetCaptcha();
        return Page();
      }

      var result = await _client.ContactUsAsync(new ContactusRequest()
      {
        FullName = FullName,
        Email = Email,
        PhoneNumber = PhoneNumber,
        Subject = Subject,
        Text = Text,
        IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
      }, cancellationToken);

      if (result.Successed)
      {
        ContactUsResult = "پیام شما با موفقیت ارسال شد و در اسرع وقت پاسخ داده خواهد شد.";
      }
      else
      {
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError("", error);
        }
      }
      return Page();
    }
  }
}
