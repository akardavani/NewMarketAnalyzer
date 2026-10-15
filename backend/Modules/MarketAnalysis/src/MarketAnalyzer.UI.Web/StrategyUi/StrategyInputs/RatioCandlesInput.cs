using MarketAnalyzer.Analysis.Infrastructure;

namespace MarketAnalyzer.UI.Web
{
    public class RatioCandlesInput : IStrategyInputDefinition
    {
        public string Strategy => StrategyName.RatioCandles;

        public async Task<StrategyInputDefinition> Build(StrategyOptionProvider options)
        {
            return new StrategyInputDefinition
            {
                Strategy = Strategy,
                Title = "نسبت نمادها",

                Parameters =
                {
                    StrategyParameterFactory.Select(
                        name: "Symbols",
                        label: "نمادها",
                        options: await options.GetSymbolOptions(),
                        defaultValue: "دلار",
                        isMultiple : true,
                        isRequired: true),

                    StrategyParameterFactory.Date(
                        name: "StartDate",
                        label: "Start Date",
                        defaultValue: options.DaysAgoShamsi(30),
                        isRequired: true
                    ),

                    StrategyParameterFactory.Date(
                        name: "EndDate",
                        label: "End Date",
                        defaultValue: options.TodayShamsi(),
                        isRequired: true
                    ),

                    StrategyParameterFactory.Select(
                        name: "Timeframe",
                        label: "Timeframe",
                        options: options.GetTimeframeOptions(),
                        defaultValue: "Daily",
                        isRequired: true)

                }
            };
        }
    }

}