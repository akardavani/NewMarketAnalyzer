using MarketAnalyzer.Analysis.Infrastructure;

namespace MarketAnalyzer.UI.Web
{
    public class YtmFutureInput : IStrategyInputDefinition
    {
        public string Strategy => StrategyName.YtmFuture;

        private List<StrategyParameterOption> GetAllSearchTypes()
        {
            return StrategyOptionFactory.List(
                StrategyOptionFactory.Item("NearestYtm", "سر رسید نزدیک YTM"),
                StrategyOptionFactory.Item("SecondYtm", "سر رسید دوم YTM"),
                StrategyOptionFactory.Item("LatestYtm", "سر رسید دور YTM"),
                StrategyOptionFactory.Item("NearestContract", "سر رسید نزدیک"),
                StrategyOptionFactory.Item("SecondContract", "سر رسید دوم"),
                StrategyOptionFactory.Item("LatestContract", "سر رسید دور"),
                StrategyOptionFactory.Item("Sood", "سود"));
        }

        private List<StrategyParameterOption> GetAllSymbols()
        {
            return StrategyOptionFactory.List(
                StrategyOptionFactory.Item("ETC", "طلای لوتوس"),
                StrategyOptionFactory.Item("SIL", "سپرده نقره")
            );
        }

        public Task<StrategyInputDefinition> Build(StrategyOptionProvider options)
        {
            var symbols = GetAllSymbols();
            var searchTypes = GetAllSearchTypes();
            
            var definition = new StrategyInputDefinition
            {
                Strategy = Strategy,
                Title = "YTM پیدا کردن ",

                Parameters =
                {
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
                        name: "Symbol",
                        label: "نماد",
                        options: symbols,
                        defaultValue: "ETC",
                        isRequired: true
                    ),

                    StrategyParameterFactory.Select(
                        name: "Calculates",
                        label: "محاسبات",
                        options: searchTypes,
                        defaultValue: "NearestYtm",
                        isRequired: true
                    )
                }
            };

            return Task.FromResult(definition);
        }        
    }
}