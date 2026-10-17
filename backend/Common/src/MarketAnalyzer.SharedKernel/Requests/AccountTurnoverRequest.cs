using System;
using System.Collections.Generic;

namespace MarketAnalyzer.SharedKernel
{
    public class AccountsTurnoverRequest
    {
        /// <summary>
        /// از تاریخ
        /// </summary>
        public DateOnly? FromDate { get; init; }
        /// <summary>
        /// تا تاریخ
        /// </summary>
        public required DateOnly ToDate { get; init; }
        /// <summary>
        /// حسابهای مورد نظر
        /// </summary>
        public IReadOnlyCollection<Item> Items { get; init; }

        public class Item
        {
            /// <summary>
            /// کد حساب
            /// </summary>
            public required AccountValue Account { get; init; }
            /// <summary>
            /// کد حساب تفصیلی
            /// </summary>
            public string DetailAccountCode { get; init; }
        }
    }
}
