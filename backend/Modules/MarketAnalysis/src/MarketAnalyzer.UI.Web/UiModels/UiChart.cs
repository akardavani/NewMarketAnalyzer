namespace MarketAnalyzer.UI.Web
{
    public class UiChart
    {
        public string Id { get; set; } = $"chart_{Guid.NewGuid():N}";

        public string Title { get; set; } = "";

        public UiChartType ChartType { get; set; }

        public List<string> Labels { get; set; } = new();

        public List<UiChartDataSet> Datasets { get; set; } = new();

        public object? Options { get; set; }
    }
}
