using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MarketAnalyzer.Portal.Pages
{
  [AllowAnonymous]
  public class ResetPasswordResultModel : PageModel
  {
    public void OnGet()
    {
    }
  }
}
