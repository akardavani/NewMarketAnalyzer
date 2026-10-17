using MarketAnalyzer.AnalysisStrategy;

namespace MarketAnalyzer.UI.Web
{
    public interface IStrategyUiAdapter
    {
        string Strategy { get; }

        UiPageResult Convert(AnalysisResult result);
    }
}
