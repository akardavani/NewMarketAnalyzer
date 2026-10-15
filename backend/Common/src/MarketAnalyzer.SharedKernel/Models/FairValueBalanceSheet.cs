using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.SharedKernel
{
    public class FairValueBalanceSheet
    {
        /// <summary>
        /// سرمایه گذاری‌ها
        /// </summary>
        public required IReadOnlyCollection<Item> Investments { get; init; }
        /// <summary>
        /// دارایی‌ها
        /// </summary>
        public required IReadOnlyCollection<Item> Assets { get; init; }
        /// <summary>
        /// بدهی‌ها
        /// </summary>
        public required IReadOnlyCollection<Item> Liabilities { get; init; }
        /// <summary>
        /// خالص دارایی‌ها
        /// </summary>
        public required IReadOnlyCollection<Item> NetAssets { get; init; }

        /// <summary>
        /// خالص دارایی
        /// </summary>
        public decimal Nav => TotalInvestments + TotalAssets - TotalLiabilities;
        /// <summary>
        /// جمع سرمایه‌گذاری‌ها
        /// </summary>
        public decimal TotalInvestments => Investments.Sum(e => e.Balance);
        /// <summary>
        /// جمع سایر دارایی‌ها
        /// </summary>
        public decimal TotalAssets => Assets.Sum(e => e.Balance);
        /// <summary>
        /// جمع بدهی‌ها
        /// </summary>
        public decimal TotalLiabilities => Liabilities.Sum(e => e.Balance);
        /// <summary>
        /// جمع خالص دارایی‌ها
        /// </summary>
        public decimal TotalNetAssets => NetAssets.Sum(e => e.Balance);

        public class Item
        {
            public required string Title { get; init; }
            public required decimal Balance { get; init; }
        }
    }
}
