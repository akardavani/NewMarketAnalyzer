using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Cms.Application
{
    public class UpdateNewsCommand : IRequest<ApiResult<int>>
    {
        public required int Id { get; init; }
        public required string Title { get; init; }
        public string Source { get; init; }
        public required string Summary { get; init; }
        public required string Text { get; init; }
        public required int OperatorId { get; init; }
    }
}
