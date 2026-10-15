using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Cms.Application
{
    public class DeleteStatuteCommand : IRequest<bool>
    {
        /// <summary>
        /// شناسه
        /// </summary>
        public required int Id { get; init; }        
    }
}