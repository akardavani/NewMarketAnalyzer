namespace MarketIntelligence.Application
{
    public sealed class FeatureSet
    {
        private readonly Dictionary<string, decimal> _values = new();

        public void Add(string name, decimal value) => _values[name] = value;

        public decimal Get(string name) => _values.TryGetValue(name, out var v) ? v : 0m;

        public IReadOnlyDictionary<string, decimal> All => _values;
    }
}
