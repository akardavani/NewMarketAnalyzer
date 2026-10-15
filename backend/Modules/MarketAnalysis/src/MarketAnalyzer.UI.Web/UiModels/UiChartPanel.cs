namespace MarketAnalyzer.UI.Web
{
    public class UiChartPanel
    {
        public string Title { get; set; } = "";

        public int Height { get; set; } = 200;

        public UiChart Chart { get; set; } = new();
    }
}
