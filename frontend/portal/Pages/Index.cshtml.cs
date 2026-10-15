using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MarketAnalyzer.ApiClient;
using MarketAnalyzer.ApiClient.Models;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Portal.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly FundApiClient _client;

        public PortalHomePageViewData Data { get; private set; }

        [BindProperty(SupportsGet = true)]
        public DateOnly? NavsFromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateOnly? NavsToDate { get; set; }

        public decimal FundReturn { get; set; }

        public string Navs { get; set; }

        public string AssetCompositionChartData { get; set; }

        public string SectorsAssetsCompositionData { get; set; }

        public IndexModel(FundApiClient client)
        {
            _client = client;
        }

        public async Task OnGetAsync(CancellationToken cancellation)
        {
            var result = await _client.GetHomePageViewDataAsync(new PageViewDataRequest()
            {
                NavsFromDate = NavsFromDate,
                NavsToDate = NavsToDate,
            }, cancellation);
            if (result.Successed)
            {
                Data = result.Data;
                Navs = JsonSerializer.Serialize(Data.Navs.Items);
                AssetCompositionChartData = JsonSerializer.Serialize(Data.AssetsComposition);
                SectorsAssetsCompositionData = JsonSerializer.Serialize(Data.SectorsAssetsComposition);
                FundReturn = Data.Navs.FundReturn;
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
        }
    }
}
