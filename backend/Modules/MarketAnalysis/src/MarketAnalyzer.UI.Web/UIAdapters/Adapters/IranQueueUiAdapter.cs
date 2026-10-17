using MarketAnalyzer.AnalysisStrategy;
using MarketAnalyzer.Analysis.Infrastructure;
using System.Text.Json;

namespace MarketAnalyzer.UI.Web
{
    public class IranQueueUiAdapter : IStrategyUiAdapter
    {
        public string Strategy => StrategyName.IranQueue;

        public UiPageResult Convert(AnalysisResult result)
        {
            var json = JsonSerializer.Serialize(result.Data);

            var data = JsonSerializer.Deserialize<IranQueueDto>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;

            var page = new UiPageResult
            {
                Strategy = result.Strategy
            };

            page.Sections.Add(CreateTopPicks(data));

            return page;
        }

        private UiSection CreateTopPicks(IranQueueDto data)
        {
            var table = new UiTable
            {
                Columns = new List<string>
                {
                    "Symbol",                    
                    "VolumeToday",
                    "DemandVolume",
                    "Ratio",
                    "Return1Month",
                    "AvgVolume30",
                    "Links"
                },

                Rows = data.QueueResult.Select(x => new Dictionary<string, object?>
                {
                    ["Symbol"] = x.Symbol,
                    ["VolumeToday"] = x.VolumeToday,
                    ["DemandVolume"] = x.DemandVolume,
                    ["Ratio"] = x.Ratio,
                    ["Return1Month"] = x.Return1Month,
                    ["AvgVolume30"] = x.AvgVolume30,

                    // قرار دادن چندین لینک در قالب یک لیست درون یک کلید واحد
                    ["Links"] = new List<UiLink>
            {
                new UiLink
                {
                    Url = $"https://www.nahayatnegar.com/tv/{x.InstrumentId}",
                    DisplayText = "چارت نهایت‌نگار",
                    Icon = "📊",
                    CssClass = "text-decoration-none fs-5 me-2" // me-2 برای ایجاد فاصله راست در بوت‌استرپ
                },
                new UiLink
                {
                    Url = $"https://tsetmc.com/instInfo/{x.InsCode}",
                    DisplayText = "صفحه TSETMC",
                    Icon = "🏛️",
                    CssClass = "text-decoration-none fs-5"
                }
            }
                }).ToList()
            };

            return new UiSection
            {
                Title = "Top Picks",
                Type = UiSectionType.Table,
                Data = table
            };
        }        
    }
}
