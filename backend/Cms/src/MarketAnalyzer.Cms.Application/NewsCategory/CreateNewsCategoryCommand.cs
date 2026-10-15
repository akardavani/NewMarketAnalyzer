using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Cms.Application
{
    public class CreateNewsCategoryCommand : IRequest<ApiResult<int>>
    {
        public required string Name { get; init; }
        public string Description { get; init; }
        public required int OperatorId { get; init; }
    }
}
