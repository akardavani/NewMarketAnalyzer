using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Cms.Application
{
    public class UpdateNewsCategoryCommand : IRequest<ApiResult<int>>
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public string Description { get; init; }
        public required int OperatorId { get; init; }
    }
}
