namespace MarketAnalyzer.UI.Web
{
    public interface IStrategyInputDefinition
    {
        string Strategy { get; }

        Task<StrategyInputDefinition> Build(StrategyOptionProvider options);
    }

}