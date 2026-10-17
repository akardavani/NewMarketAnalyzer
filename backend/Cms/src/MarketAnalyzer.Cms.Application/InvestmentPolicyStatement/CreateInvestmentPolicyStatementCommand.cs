using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.Cms.Application
{
    public class CreateInvestmentPolicyStatementCommand : IRequest<int>
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
        /// فعال/غیرفعال
        /// </summary>
        public required bool IsActive { get; init; }
        /// <summary>
        /// محتوای فایل بیانیه سیاست سرمایه‌گذاری
        /// </summary>
        public required FileContent FileContent { get; init; }

        public required int OperatorId { get; init; }
    }
}