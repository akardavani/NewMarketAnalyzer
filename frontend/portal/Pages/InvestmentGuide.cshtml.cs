using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MarketAnalyzer.Portal.Pages
{
  [AllowAnonymous]
  public class InvestmentGuideModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
