namespace MarketAnalyzer.UI.Web
{
    public class StrategyParameterDefinition
    {
        public string Name { get; set; } = "";

        public string Label { get; set; } = "";

        public StrategyParameterType Type { get; set; }

        public object? DefaultValue { get; set; }

        public bool IsRequired { get; set; } = true;

        public List<StrategyParameterOption> Options { get; set; } = new();
        public bool IsMultiple { get; set; }

    }
}
