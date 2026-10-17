namespace MarketAnalyzer.UI.Web
{
    public static class StrategyParameterFactory
    {
        public static StrategyParameterDefinition Text(
            string name,
            string label,
            object? defaultValue = null,
            bool isRequired = false)
        {
            return new StrategyParameterDefinition
            {
                Name = name,
                Label = label,
                Type = StrategyParameterType.Text,
                DefaultValue = defaultValue,
                IsRequired = isRequired
            };
        }

        public static StrategyParameterDefinition Number(
            string name,
            string label,
            object? defaultValue = null,
            bool isRequired = false)
        {
            return new StrategyParameterDefinition
            {
                Name = name,
                Label = label,
                Type = StrategyParameterType.Number,
                DefaultValue = defaultValue,
                IsRequired = isRequired
            };
        }

        public static StrategyParameterDefinition Date(
            string name,
            string label,
            object? defaultValue = null,
            bool isRequired = false)
        {
            return new StrategyParameterDefinition
            {
                Name = name,
                Label = label,
                Type = StrategyParameterType.Date,
                DefaultValue = defaultValue,
                IsRequired = isRequired
            };
        }

        public static StrategyParameterDefinition Boolean(
            string name,
            string label,
            object? defaultValue = null,
            bool isRequired = false)
        {
            return new StrategyParameterDefinition
            {
                Name = name,
                Label = label,
                Type = StrategyParameterType.Boolean,
                DefaultValue = defaultValue,
                IsRequired = isRequired
            };
        }

        public static StrategyParameterDefinition Select(
            string name,
            string label,
            IEnumerable<StrategyParameterOption> options,
            object? defaultValue = null,
            bool isRequired = false,
            bool isMultiple = false)
        {
            return new StrategyParameterDefinition
            {
                Name = name,
                Label = label,
                Type = StrategyParameterType.Select,
                DefaultValue = defaultValue,
                IsRequired = isRequired,
                IsMultiple = isMultiple,
                Options = options.ToList()
            };
        }


        public static StrategyParameterDefinition Timeframe(
            string name,
            string label,
            IEnumerable<StrategyParameterOption> options,
            object? defaultValue = null,
            bool isRequired = false)
        {
            return new StrategyParameterDefinition
            {
                Name = name,
                Label = label,
                Type = StrategyParameterType.Timeframe,
                DefaultValue = defaultValue,
                IsRequired = isRequired,
                Options = options.ToList()
            };
        }

        public static StrategyParameterDefinition File(
            string name,
            string label,
            bool isRequired = false)
        {
            return new StrategyParameterDefinition
            {
                Name = name,
                Label = label,
                Type = StrategyParameterType.File,
                IsRequired = isRequired
            };
        }

        public static StrategyParameterDefinition Files(
            string name,
            string label,
            bool isRequired = false)
        {
            return new StrategyParameterDefinition
            {
                Name = name,
                Label = label,
                Type = StrategyParameterType.Files,
                IsRequired = isRequired
            };
        }
    }
}
