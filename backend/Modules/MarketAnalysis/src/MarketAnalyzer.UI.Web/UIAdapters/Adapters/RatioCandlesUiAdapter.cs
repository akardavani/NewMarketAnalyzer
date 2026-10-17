using MarketAnalyzer.AnalysisStrategy;
using MarketAnalyzer.Analysis.Infrastructure;
using System.Text.Json;

namespace MarketAnalyzer.UI.Web
{
    public class RatioCandlesUiAdapter : IStrategyUiAdapter
    {
        public string Strategy => StrategyName.RatioCandles;

        public UiPageResult Convert(AnalysisResult result)
        {
            var json = JsonSerializer.Serialize(result.Data);

            var series = JsonSerializer.Deserialize<List<CandleSeriesResult>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new();

            var page = new UiPageResult
            {
                Strategy = result.Strategy
            };

            page.Sections.Add(CreateChart(series));

            return page;
        }

        private UiSection CreateChart(List<CandleSeriesResult> series)
        {
            var datasets = new List<UiChartDataSet>();

            foreach (var item in series)
            {
                datasets.Add(new UiChartDataSet
                {
                    Label = item.Symbol,
                    Data = item.Results
                        .Select(x => (double)x.Growth)
                        .ToList(),

                    BorderColor = GetColor(item.Symbol),
                    BackgroundColor = "rgba(0,0,0,0)",
                    Fill = false
                });
            }

            var labels = series
                .FirstOrDefault()?
                .Results
                .Select(x => x.PersianDate)
                .ToList() ?? new();

            var chart = new UiChart
            {
                ChartType = UiChartType.Line,
                Labels = labels,
                Datasets = datasets
            };

            return new UiSection
            {
                Title = "Growth Chart",
                Type = UiSectionType.Chart,
                Data = chart
            };
        }

        private string GetColor(string key)
        {
            var colors = new[]
            {
                "rgba(31,119,180,1)",   // blue
                "rgba(214,39,40,1)",    // red
                "rgba(44,160,44,1)",    // green
                "rgba(148,103,189,1)",  // purple
                "rgba(255,127,14,1)",   // orange
                "rgba(23,190,207,1)",   // cyan
                "rgba(140,86,75,1)",    // brown
                "rgba(227,119,194,1)",  // pink
                "rgba(127,127,127,1)",  // gray
                "rgba(188,189,34,1)",   // olive
                "rgba(0,128,255,1)",    // bright blue
                "rgba(255,0,128,1)"     // magenta
            };


            var index = Math.Abs(key.GetHashCode()) % colors.Length;

            return colors[index];
        }

    }
}
