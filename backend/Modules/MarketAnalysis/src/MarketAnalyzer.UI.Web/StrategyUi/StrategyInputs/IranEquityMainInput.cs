using MarketAnalyzer.Analysis.Infrastructure;

namespace MarketAnalyzer.UI.Web
{
    public class IranEquityMainInput : IStrategyInputDefinition
    {
        public string Strategy => StrategyName.IranEquityMain;

        public async Task<StrategyInputDefinition> Build(StrategyOptionProvider options)
        {
            return new StrategyInputDefinition
            {
                Strategy = Strategy,
                Title = "پیدا کردن سهم با اکسل ایزی",

                Parameters =
                {
                    StrategyParameterFactory.File(
                        name: "InputFilePath",
                        label: "فایل اکسل",
                        isRequired: true),

                    //StrategyParameterFactory.Select(
                    //    name: "Market",
                    //    label: "Market",
                    //    options: options.GetMarketOptions(),
                    //    defaultValue: "Stock",
                    //    isRequired: true),

                    //StrategyParameterFactory.Select(
                    //    name: "BaseSymbolMarket",
                    //    label: "Base Symbol Market",
                    //    options: options.GetMarketOptions(),
                    //    defaultValue: "Macro",
                    //    isRequired: true),

                    //StrategyParameterFactory.Select(
                    //    name: "BaseSymbol",
                    //    label: "Base Symbol",
                    //    options: await options.GetSymbolOptions(),
                    //    defaultValue: "USDTIRT",
                    //    isRequired: true),

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

                    //StrategyParameterFactory.Select(
                    //    name: "Timeframe",
                    //    label: "Timeframe",
                    //    options: options.GetTimeframeOptions(),
                    //    defaultValue: "Daily",
                    //    isRequired: true)
                }
            };
        }
    }

}
