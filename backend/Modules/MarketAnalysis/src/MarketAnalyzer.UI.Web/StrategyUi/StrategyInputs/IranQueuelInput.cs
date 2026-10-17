using MarketAnalyzer.Analysis.Infrastructure;

namespace MarketAnalyzer.UI.Web
{
    public class IranQueuelInput : IStrategyInputDefinition
    {
        public string Strategy => StrategyName.IranQueue;

        public async Task<StrategyInputDefinition> Build(StrategyOptionProvider options)
        {
            return new StrategyInputDefinition
            {
                Strategy = Strategy,
                Title = "صف خرید",

                Parameters =
                {
                    StrategyParameterFactory.File(
                        name: "InputFilePath",
                        label: "فایل اکسل",
                        isRequired: true),

                    StrategyParameterFactory.Text(
                        name: "FromRatio",
                        label: "از نسبت",
                        defaultValue: 5,
                        isRequired: true
                    ),

                    StrategyParameterFactory.Text(
                        name: "ToRatio",
                        label: "تا نسبت",
                        defaultValue: 15,
                        isRequired: true
                    ),

                    StrategyParameterFactory.Text(
                        name: "Return1Month",
                        label: "بازدهی1ماهه",
                        defaultValue: 50,
                        isRequired: true
                    )
                }
            };
        }
    }
}