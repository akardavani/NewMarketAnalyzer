using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MarketAnalyzer.ApiClient;
using MarketAnalyzer.ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MarketAnalyzer.Portal.Pages
{
    [AllowAnonymous]
    public class FundStatusModel : PageModel
    {
        private readonly FundApiClient _client;

        [BindProperty(SupportsGet = true)]
        public string FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ToDate { get; set; }


        public List<FundStatusReturnResult> FundStatusReturns { get; private set; }

        public FundStatusModel(FundApiClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> OnGetAsync(CancellationToken cancellation)
        {
            var to = ToDate.FromPersian();
            var from = FromDate.FromPersian();

            if (to is null)
            {
                var homeResponse = await _client.GetHomePageViewDataAsync(
                    new PageViewDataRequest
                    {

            },
                    cancellation);

                if (!homeResponse.Successed)
                {
                    foreach (var error in homeResponse.Errors)
                        ModelState.AddModelError("", error);

                    return Page();
                }

                to = homeResponse.Data.LastNavDate; 
            }

            from ??= to.Value.AddDays(-7);

            FromDate = from.Value.ToPersian();
            ToDate = to.Value.ToPersian();

            var response = await _client.FundStatusReturn(from.Value, to.Value, cancellation);
            if (response.Successed)
            {
                FundStatusReturns = response.Data;
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
    }
}
