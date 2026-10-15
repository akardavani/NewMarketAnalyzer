using System;

namespace MarketAnalyzer.Cms.Application
{
    public class NewsListQueryResult
    {
        /// <summary>
        /// شناسه خبر
        /// </summary>
        public required int Id { get; init; }
        /// <summary>
        /// عنوان
        /// </summary>
        public required string Title { get; init; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public required DateOnly Date { get; init; }
    }
}
