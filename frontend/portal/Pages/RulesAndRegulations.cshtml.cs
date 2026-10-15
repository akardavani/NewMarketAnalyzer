using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MarketAnalyzer.Portal.Pages
{
  [AllowAnonymous]
  public class RulesAndRegulationsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
