using System;

namespace MarketAnalyzer.Cms.Application
{
    public class PortalInvestmentPolicyStatementQueryResult
    {
        /// <summary>
        /// شناسه بیانیه سیاست سرمایه‌گذاری
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
