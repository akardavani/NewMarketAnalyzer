using MarketAnalyzer.UI.Web;
using System.Text.Json;

namespace MarketAnalyzer.AnalysisStrategy
{
    public class MultiIndicatorDivergenceUiAdapter : IStrategyUiAdapter
    {
        // اطمینان از همخوانی نام استراتژی با کلاس اصلی
        public string Strategy => "MultiIndicatorDivergence";

        public UiPageResult Convert(AnalysisResult result)
        {
            var json = JsonSerializer.Serialize(result.Data);

            var data = JsonSerializer.Deserialize<DivergenceMainDto>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;

            var page = new UiPageResult
            {
                Strategy = result.Strategy
            };

            // افزودن بخش‌های مختلف صفحه خروجی
            page.Sections.Add(CreateMetrics(data));
            page.Sections.Add(CreateTopPicksTable(data));
            page.Sections.Add(CreateDetailedDivergenceTable(data));

            return page;
        }

        // ۱. بخش آمارهای خلاصه بازار
        private UiSection CreateMetrics(DivergenceMainDto data)
        {
            int totalBullish = data.TopPicks.Count(x => x.MarketPosition.Contains("کف"));
            int totalBearish = data.TopPicks.Count(x => x.MarketPosition.Contains("سقف"));

            return new UiSection
            {
                Title = "خلاصه وضعیت واگرایی بازار",
                Type = UiSectionType.Metrics,
                Data = new List<UiMetric>
                {
                    new UiMetric
                    {
                        Label = "کل نمادهای بررسی شده",
                        Value = data.TotalSymbolsAnalyzed.ToString()
                    },
                    new UiMetric
                    {
                        Label = "فرصت‌های خرید (واگرایی کف)",
                        Value = totalBullish.ToString()
                    },
                    new UiMetric
                    {
                        Label = "هشدارهای فروش (واگرایی سقف)",
                        Value = totalBearish.ToString()
                    },
                    new UiMetric
                    {
                        Label = "زمان تحلیل بازار",
                        Value = data.MarketTimestamp.ToString("yyyy-MM-dd HH:mm")
                    }
                }
            };
        }

        // ۲. جدول کلی نمادهای دارای واگرایی چندگانه
        private UiSection CreateTopPicksTable(DivergenceMainDto data)
        {
            var table = new UiTable
            {
                Columns = new List<string>
                {
                    "نماد",
                    "آخرین قیمت",
                    "تعداد واگرایی فعال",
                    "موقعیت بازار",
                    "ارزش ذاتی",
                    "فاصله تا حمایت (%)",
                    "فاصله تا مقاومت (%)",
                    "اندیکاتورهای فعال"
                },

                Rows = data.TopPicks.Select(x => new Dictionary<string, object?>
                {
                    ["نماد"] = x.Symbol,
                    ["آخرین قیمت"] = x.LastPrice.ToString("N0"),
                    ["تعداد واگرایی فعال"] = x.DivergenceCount,
                    ["موقعیت بازار"] = x.MarketPosition,
                    ["ارزش ذاتی"] = x.FairValue > 0 ? x.FairValue.ToString("N0") : "نامشخص",
                    ["فاصله تا حمایت (%)"] = x.DistanceToSupport.ToString("P1"),
                    ["فاصله تا مقاومت (%)"] = x.DistanceToResistance.ToString("P1"),
                    ["اندیکاتورهای فعال"] = string.Join("، ", x.DetectedDivergences.Select(d => d.IndicatorName))
                }).ToList()
            };

            return new UiSection
            {
                Title = "نمادهای برتر با واگرایی چندگانه (حداقل ۲ واگرایی همزمان)",
                Type = UiSectionType.Table,
                Data = table
            };
        }

        // ۳. جدول جزئیات واگرایی‌ها به تفکیک اندیکاتور و فاصله زمانی وقوع (BarsAgo)
        private UiSection CreateDetailedDivergenceTable(DivergenceMainDto data)
        {
            var detailedRows = new List<Dictionary<string, object?>>();

            foreach (var pick in data.TopPicks)
            {
                foreach (var div in pick.DetectedDivergences)
                {
                    detailedRows.Add(new Dictionary<string, object?>
                    {
                        ["نماد"] = pick.Symbol,
                        ["اندیکاتور"] = div.IndicatorName,
                        ["نوع واگرایی"] = div.Type,
                        ["فاصله از پیوت (کندل)"] = div.BarsAgo,
                        ["آخرین قیمت"] = pick.LastPrice.ToString("N0"),
                        ["ATR (۱۴)"] = pick.ATR.ToString("N1")
                    });
                }
            }

            var table = new UiTable
            {
                Columns = new List<string>
                {
                    "نماد",
                    "اندیکاتور",
                    "نوع واگرایی",
                    "فاصله از پیوت (کندل)",
                    "آخرین قیمت",
                    "ATR (۱۴)"
                },
                Rows = detailedRows.OrderBy(r => r["نماد"]).ToList()
            };

            return new UiSection
            {
                Title = "جزئیات واگرایی‌های شناسایی شده به تفکیک اندیکاتور",
                Type = UiSectionType.Table,
                Data = table
            };
        }
    }

    public class DivergenceMainDto
    {
        public DateTime MarketTimestamp { get; set; }
        public int TotalSymbolsAnalyzed { get; set; }
        public List<DivergencePickDto> TopPicks { get; set; } = new();
    }

    public class DivergencePickDto
    {
        public string Symbol { get; set; }
        public decimal LastPrice { get; set; }
        public decimal ATR { get; set; }
        public decimal FairValue { get; set; }
        public decimal DistanceToSupport { get; set; }
        public decimal DistanceToResistance { get; set; }
        public List<DivergenceDetectionDto> DetectedDivergences { get; set; } = new();
        public int DivergenceCount => DetectedDivergences.Count;
        public string MarketPosition { get; set; }
    }

    public class DivergenceDetectionDto
    {
        public string IndicatorName { get; set; }
        public string Type { get; set; }
        public int BarsAgo { get; set; }
    }
}
