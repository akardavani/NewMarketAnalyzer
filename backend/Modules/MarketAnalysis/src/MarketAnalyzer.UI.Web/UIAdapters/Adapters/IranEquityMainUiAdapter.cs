using MarketAnalyzer.AnalysisStrategy;
using MarketAnalyzer.Analysis.Infrastructure;
using System.Text.Json;

namespace MarketAnalyzer.UI.Web
{
    public class IranEquityMainUiAdapter : IStrategyUiAdapter
    {
        public string Strategy => StrategyName.IranEquityMain;

        public UiPageResult Convert(AnalysisResult result)
        {
            var json = JsonSerializer.Serialize(result.Data);

            var data = JsonSerializer.Deserialize<IranEquityMainDto>(
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
            page.Sections.Add(CreateLeadingSectors(data));
            page.Sections.Add(CreateMetrics(data));
            page.Sections.Add(CreateSectorReport(data));

            return page;
        }

        private UiSection CreateMetrics(IranEquityMainDto data)
        {
            return new UiSection
            {
                Title = "Market Summary",
                Type = UiSectionType.Metrics,
                Data = new List<UiMetric>
                {
                    new UiMetric
                    {
                        Label = "Total Symbols",
                        Value = data.TotalSymbolsAnalyzed.ToString()
                    },
                    new UiMetric
                    {
                        Label = "Market Time",
                        Value = data.MarketTimestamp.ToString("yyyy-MM-dd HH:mm")
                    }
                }
            };
        }

        private UiSection CreateTopPicks(IranEquityMainDto data)
        {
            var table = new UiTable
            {
                Columns = new List<string>
                {
                    "Symbol",
                    "Score",
                    "Percentile",
                    "Tier",
                    "Price"
                },

                Rows = data.TopPicks.Select(x => new Dictionary<string, object?>
                {
                    ["Symbol"] = x.Symbol,
                    ["Score"] = x.FinalScore,
                    ["Percentile"] = x.Percentile,
                    ["Tier"] = x.Tier,
                    ["Price"] = x.LastPrice
                }).ToList()
            };

            return new UiSection
            {
                Title = "Top Picks",
                Type = UiSectionType.Table,
                Data = table
            };
        }

        private UiSection CreateLeadingSectors(IranEquityMainDto data)
        {
            var table = new UiTable
            {
                Columns = new List<string>
                {
                    "Sector",
                    "AverageScore",
                    "MomentumScore",
                    "TopTierCount"
                },

                Rows = data.LeadingSectors.Select(x => new Dictionary<string, object?>
                {
                    ["Sector"] = x.SectorName,
                    ["AverageScore"] = x.AverageScore,
                    ["MomentumScore"] = x.MomentumScore,
                    ["TopTierCount"] = x.TopTierCount
                }).ToList()
            };

            return new UiSection
            {
                Title = "Leading Sectors",
                Type = UiSectionType.Table,
                Data = table
            };
        }

        private UiSection CreateSectorReport(IranEquityMainDto data)
        {
            var table = new UiTable
            {
                Columns = new List<string>
                {
                    "Sector",
                    "AverageScore",
                    "MomentumScore",
                    "TotalMoneyFlow",
                    "TopTierCount",
                    "Status",
                    "Symbols"
                },

                Rows = data.FullSectorReport.Select(x => new Dictionary<string, object?>
                {
                    ["Sector"] = x.SectorName,
                    ["AverageScore"] = x.AverageScore,
                    ["MomentumScore"] = x.MomentumScore,
                    ["TotalMoneyFlow"] = x.TotalMoneyFlow,
                    ["TopTierCount"] = x.TopTierCount,
                    ["Status"] = x.Status,
                    ["Symbols"] = x.SymbolCount
                }).ToList()
            };

            return new UiSection
            {
                Title = "Full Sector Report",
                Type = UiSectionType.Table,
                Data = table
            };
        }


        //private UiSection CreateTechnicalChart(StockHistory data)
        //{
        //    var layout = new UiChartLayout
        //    {
        //        Title = "Technical Analysis"
        //    };

        //    layout.Panels.Add(CreatePricePanel(data));

        //    layout.Panels.Add(CreateVolumePanel(data));

        //    layout.Panels.Add(CreateRsiPanel(data));

        //    layout.Panels.Add(CreateMacdPanel(data));

        //    return new UiSection
        //    {
        //        Title = "Charts",
        //        Type = UiSectionType.Chart,
        //        Data = layout
        //    };
        //}

        //private UiChartPanel CreatePricePanel(StockHistory data)
        //{
        //    var candles = data.Candles.Select(x => new
        //    {
        //        x = x.Date,
        //        o = x.Open,
        //        h = x.High,
        //        l = x.Low,
        //        c = x.Close
        //    }).ToList();

        //    return new UiChartPanel
        //    {
        //        Height = 400,

        //        Chart = new UiChart
        //        {
        //            ChartType = UiChartType.Candlestick,

        //            Datasets =
        //    {
        //        new UiChartDataset
        //        {
        //            Label = "Price",
        //            Data = candles.Cast<object>().ToList()
        //        }
        //    }
        //        }
        //    };
        //}

        //private UiChartPanel CreateRsiPanel(StockHistory data)
        //{
        //    return new UiChartPanel
        //    {
        //        Height = 150,

        //        Chart = new UiChart
        //        {
        //            ChartType = UiChartType.Line,

        //            Labels = data.Dates,

        //            Datasets =
        //    {
        //        new UiChartDataset
        //        {
        //            Label = "RSI",
        //            Data = data.Rsi.Cast<object>().ToList(),
        //            BorderColor = "orange"
        //        }
        //    }
        //        }
        //    };
        //}

        //private UiChartPanel CreateMacdPanel(StockHistory data)
        //{
        //    return new UiChartPanel
        //    {
        //        Height = 200,

        //        Chart = new UiChart
        //        {
        //            ChartType = UiChartType.Line,

        //            Labels = data.Dates,

        //            Datasets =
        //    {
        //        new UiChartDataset
        //        {
        //            Label = "MACD",
        //            Data = data.Macd.Cast<object>().ToList(),
        //            BorderColor = "blue"
        //        },
        //        new UiChartDataset
        //        {
        //            Label = "Signal",
        //            Data = data.MacdSignal.Cast<object>().ToList(),
        //            BorderColor = "red"
        //        }
        //    }
        //        }
        //    };
        //}

    }
}
