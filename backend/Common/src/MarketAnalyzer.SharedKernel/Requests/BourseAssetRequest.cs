using System;

namespace MarketAnalyzer.SharedKernel
{
    public class BourseAssetRequest
    {
        /// <summary>
        /// تاریخ
        /// </summary>
        public required DateOnly Date { get; init; }
        /// <summary>
        /// ارزش کل دارایی‌ها
        /// </summary>
        public required decimal Total { get; init; }
    }
}
