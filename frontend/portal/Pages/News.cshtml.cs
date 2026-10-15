using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MarketAnalyzer.Portal.Pages
{
  [AllowAnonymous]
  public class NewsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
