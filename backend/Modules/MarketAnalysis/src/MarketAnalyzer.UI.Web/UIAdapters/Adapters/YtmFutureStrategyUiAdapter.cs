using MarketAnalyzer.AnalysisStrategy;
using MarketAnalyzer.Analysis.Infrastructure;
using System.Text.Json;

namespace MarketAnalyzer.UI.Web
{
    public class YtmFutureStrategyUiAdapter : IStrategyUiAdapter
    {
        public string Strategy => StrategyName.YtmFuture;

        public UiPageResult Convert(AnalysisResult result)
        {
            var json = JsonSerializer.Serialize(result.Data);

            var points = JsonSerializer.Deserialize<List<CandleResult>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new();

            var data = new YtmFutureDto
            {
                BaseSymbol = "Spot",
                CompareSymbol = "Future",
                StartDate = points.Count > 0 ? points.First().Date.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue,
                EndDate = points.Count > 0 ? points.Last().Date.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue,
                Points = points.Select(x => new YtmFuturePointDto
                {
                    Date = x.Date,
                    PersianDate = x.PersianDate,
                    Growth = x.Growth
                }).ToList()
            };

            var page = new UiPageResult
            {
                Strategy = result.Strategy
            };

            //page.Sections.Add(CreateMetrics(data));
            page.Sections.Add(CreateChart(data));
            //page.Sections.Add(CreateTable(data));

            return page;
        }


        private UiSection CreateMetrics(YtmFutureDto data)
        {
            return new UiSection
            {
                Title = "Ratio Summary",
                Type = UiSectionType.Metrics,
                Data = new List<UiMetric>
                {
                    new UiMetric
                    {
                        Label = "Base Symbol",
                        Value = data.BaseSymbol
                    },
                    new UiMetric
                    {
                        Label = "Compare Symbol",
                        Value = data.CompareSymbol
                    },
                    new UiMetric
                    {
                        Label = "Start Date",
                        Value = data.StartDate.ToString("yyyy-MM-dd")
                    },
                    new UiMetric
                    {
                        Label = "End Date",
                        Value = data.EndDate.ToString("yyyy-MM-dd")
                    },
                    new UiMetric
                    {
                        Label = "Points",
                        Value = data.Points.Count.ToString()
                    }
                }
            };
        }

        private UiSection CreateChart(YtmFutureDto data)
        {
            var chart = new UiChart
            {
                ChartType = UiChartType.Line,
                Labels = data.Points
                    .Select(x => x.PersianDate)
                    .ToList(),

                Datasets = new List<UiChartDataSet>
                {
                    new UiChartDataSet
                    {
                        Label = $"{data.CompareSymbol} / {data.BaseSymbol}",
                        Data = data.Points
                            .Select(x => (double)x.Growth)
                            .ToList(),
                        BorderColor = "rgba(75,192,192,1)",
                        BackgroundColor = "rgba(75,192,192,0.15)",
                        Fill = false
                    }
                }
            };

            return new UiSection
            {
                Title = "Ratio Chart",
                Type = UiSectionType.Chart,
                Data = chart
            };
        }

        private UiSection CreateTable(YtmFutureDto data)
        {
            var table = new UiTable
            {
                Columns = new List<string>
                {
                    "Date",
                    "PersianDate",
                    "Growth"
                },

                Rows = data.Points.Select(x => new Dictionary<string, object?>
                {
                    ["Date"] = x.Date.ToString(),
                    ["PersianDate"] = x.PersianDate,
                    ["Growth"] = x.Growth
                }).ToList()
            };

            return new UiSection
            {
                Title = "Ratio Data",
                Type = UiSectionType.Table,
                Data = table
            };
        }
    }
}
