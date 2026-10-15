using MarketAnalyzer.Abstractions;


namespace MarketAnalyzer.Cms.Application
{
    public class NewsCategoryQuery : IRequest<NewsCategoryQueryResult>
    {
        public required int Id { get; init; }
    }

}
