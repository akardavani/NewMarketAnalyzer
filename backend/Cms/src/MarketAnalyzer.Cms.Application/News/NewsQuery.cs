using MarketAnalyzer.Abstractions;


namespace MarketAnalyzer.Cms.Application
{
    public class NewsQuery : IRequest<NewsQueruResult>
    {
        public required int Id { get; init; }
    }
}
