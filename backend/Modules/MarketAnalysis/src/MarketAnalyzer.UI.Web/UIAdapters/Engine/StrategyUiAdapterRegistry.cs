using MarketAnalyzer.AnalysisStrategy;

namespace MarketAnalyzer.UI.Web
{
    public class StrategyUiAdapterRegistry
    {
        private readonly Dictionary<string, IStrategyUiAdapter> _adapters;

        public StrategyUiAdapterRegistry(IEnumerable<IStrategyUiAdapter> adapters)
        {
            _adapters = adapters.ToDictionary(x => x.Strategy);
        }

        public UiPageResult Convert(AnalysisResult result)
        {
            if (_adapters.TryGetValue(result.Strategy, out var adapter))
                return adapter.Convert(result);

            return DefaultJson(result);
        }

        private UiPageResult DefaultJson(AnalysisResult result)
        {
            return new UiPageResult
            {
                Strategy = result.Strategy,
                Sections =
            {
                new UiSection
                {
                    Title = "Raw Result",
                    Type = UiSectionType.Json,
                    Data = result.Data
                }
            }
            };
        }
    }
}
