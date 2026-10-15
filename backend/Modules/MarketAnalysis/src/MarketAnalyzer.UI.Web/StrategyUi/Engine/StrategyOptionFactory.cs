namespace MarketAnalyzer.UI.Web
{
    public static class StrategyOptionFactory
    {
        public static StrategyParameterOption Item(string value, string text)
        {
            return new StrategyParameterOption
            {
                Value = value,
                Text = text
            };
        }

        public static List<StrategyParameterOption> List(
            params StrategyParameterOption[] items)
        {
            return items.ToList();
        }

        public static List<StrategyParameterOption> FromDictionary(
            Dictionary<string, string> source)
        {
            return source
                .Select(x => new StrategyParameterOption
                {
                    Value = x.Key,
                    Text = x.Value
                })
                .ToList();
        }
    }
}
