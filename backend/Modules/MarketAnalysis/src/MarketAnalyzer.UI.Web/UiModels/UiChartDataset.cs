namespace MarketAnalyzer.UI.Web
{
    public class UiChartDataSet
    {
        public string Label { get; set; } = "";

        public List<double> Data { get; set; } = new();

        public string BorderColor { get; set; } = "#3e95cd";

        public string BackgroundColor { get; set; } = "rgba(62,149,205,0.4)";

        public bool Fill { get; set; } = false;

        public string? YAxisID { get; set; }
    }
}
