namespace MarketAnalyzer.UI.Web
{
    public class StrategyInputDefinition
    {
        public string Strategy { get; set; } = "";

        public string Title { get; set; } = "";

        public List<StrategyParameterDefinition> Parameters { get; set; } = new();
    }
}
