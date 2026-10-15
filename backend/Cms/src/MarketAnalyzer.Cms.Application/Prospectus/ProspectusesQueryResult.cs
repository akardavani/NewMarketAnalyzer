using System;

namespace MarketAnalyzer.Cms.Application
{
    public class ProspectusesQueryResult
    {
        public required int Id { get; init; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public required DateOnly Date { get; init; }
        /// <summary>
        /// شرح
        /// </summary>
        public required string Description { get; init; }
        /// <summary>
        /// پیوست
        /// </summary>
        public required string AttachmentId { get; init; }
    }
}
