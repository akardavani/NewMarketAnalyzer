using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MarketAnalyzer.Portal.Pages
{
  [AllowAnonymous]
  public class CostsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
