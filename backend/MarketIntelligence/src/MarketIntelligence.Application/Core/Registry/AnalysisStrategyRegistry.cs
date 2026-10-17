namespace MarketIntelligence.Application
{
    public class AnalysisStrategyRegistry
    {
        private readonly Dictionary<string, IAnalysisStrategy> _strategies =
            new(StringComparer.OrdinalIgnoreCase);

        public AnalysisStrategyRegistry(IEnumerable<IAnalysisStrategy> strategies)
        {
            foreach (var s in strategies)
                _strategies[s.Name] = s;
        }

        public IAnalysisStrategy Get(string name)
        {
            if (!_strategies.TryGetValue(name, out var strategy))
                throw new KeyNotFoundException($"Strategy '{name}' not found.");

            return strategy;
        }
    }
}
