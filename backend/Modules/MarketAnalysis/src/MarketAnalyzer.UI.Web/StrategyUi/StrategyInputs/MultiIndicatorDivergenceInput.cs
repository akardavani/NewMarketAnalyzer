using MarketAnalyzer.Analysis.Infrastructure;


namespace MarketAnalyzer.UI.Web
{
    public class MultiIndicatorDivergenceInput : IStrategyInputDefinition
    {
        public string Strategy => StrategyName.MultiIndicatorDivergence;

        public async Task<StrategyInputDefinition> Build(StrategyOptionProvider options)
        {
            return new StrategyInputDefinition
            {
                Strategy = Strategy,
                Title = "واگرایی اندیکاتور ها",

                Parameters =
                {
                    StrategyParameterFactory.File(
                        name: "InputFilePath",
                        label: "فایل اکسل",
                        isRequired: true),
                }
            };
        }
    }

}