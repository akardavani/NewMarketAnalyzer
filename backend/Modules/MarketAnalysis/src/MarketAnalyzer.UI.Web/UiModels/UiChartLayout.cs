namespace MarketAnalyzer.UI.Web
{
    public class UiChartLayout
    {
        public string Id { get; set; } = $"chart_{Guid.NewGuid():N}";

        public string Title { get; set; } = "";

        public List<UiChartPanel> Panels { get; set; } = new();
    }
}
