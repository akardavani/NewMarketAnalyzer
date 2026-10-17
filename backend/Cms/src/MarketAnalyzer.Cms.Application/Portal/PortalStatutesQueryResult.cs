using System;

namespace MarketAnalyzer.Cms.Application
{
    public class PortalStatutesQueryResult
    {
        /// <summary>
        /// شناسه اساسنامه
        /// </summary>
        public required int Id { get; init; }
        /// <summary>
        /// شرح
        /// </summary>
        public required string Description { get; init; }
        /// <summary>
        /// پیوست
        /// </summary>
        public required string AttachmentId { get; init; }
        /// <summary>
        /// تاریخ 
        /// </summary>
        public required DateOnly Date { get; init; }
    }
}
