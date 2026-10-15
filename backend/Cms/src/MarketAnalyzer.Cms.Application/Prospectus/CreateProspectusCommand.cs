using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.Cms.Application
{
    public class CreateProspectusCommand : IRequest<int>
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
        /// محتوای فایل امیدنامه
        /// </summary>
        public required FileContent FileContent { get; init; }
        /// <summary>
        /// شناسه اپراتور
        /// </summary>
        public required int OperatorId { get; init; }
    }
}