using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MarketAnalyzer.Portal.Pages
{
  [AllowAnonymous]
  public class FinancialStatementsModel : PageModel
  {
    public void OnGet()
    {
    }
  }
}
