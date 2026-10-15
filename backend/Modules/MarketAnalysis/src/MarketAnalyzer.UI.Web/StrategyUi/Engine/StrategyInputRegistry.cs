namespace MarketAnalyzer.UI.Web
{
    public class StrategyInputRegistry
    {
        private readonly IEnumerable<IStrategyInputDefinition> _definitions;
        private readonly StrategyOptionProvider _options;

        public StrategyInputRegistry(
            IEnumerable<IStrategyInputDefinition> definitions,
            StrategyOptionProvider options)
        {
            _definitions = definitions;
            _options = options;
        }

        public async Task<List<StrategyInputDefinition>> GetAll()
        {
            var result = new List<StrategyInputDefinition>();

            foreach (var def in _definitions)
            {
                result.Add(await def.Build(_options));
            }

            return result;
        }

        public async Task<StrategyInputDefinition?> GetByStrategy(string strategy)
        {
            var def = _definitions.FirstOrDefault(x => x.Strategy == strategy);

            if (def == null)
                return null;

            return await def.Build(_options);
        }
    }

}