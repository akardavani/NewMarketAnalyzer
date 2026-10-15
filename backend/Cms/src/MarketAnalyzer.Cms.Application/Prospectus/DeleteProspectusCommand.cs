using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Cms.Application
{
    public class DeleteProspectusCommand : IRequest<bool>
    {
        /// <summary>
        /// شناسه
        /// </summary>
        public required int Id { get; init; }
    }    
}