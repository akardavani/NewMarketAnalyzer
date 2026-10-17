using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.Cms.Application
{
    public class CreateStatuteCommand : IRequest<int>
    {
        /// <summary>
        /// تاریخ
        /// </summary>
        public required DateOnly Date { get; init; }
        /// <summary>
        /// شرح
        /// </summary>
        public required string Description { get; init; }
        /// <summary>
        /// محتوای فایل اساسنامه
        /// </summary>
        public required FileContent FileContent { get; init; }

        public required int OperatorId { get; init; }
    }
}