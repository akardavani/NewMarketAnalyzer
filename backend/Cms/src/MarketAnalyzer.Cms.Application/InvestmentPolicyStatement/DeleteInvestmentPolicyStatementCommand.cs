using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Cms.Application
{
    public class DeleteInvestmentPolicyStatementCommand : IRequest<bool>
    {
        /// <summary>
        /// شناسه
        /// </summary>
        public required int Id { get; init; }
    }
}