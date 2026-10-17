using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.SharedKernel
{
    public class IncomeStatement
    {
        /// <summary>
        /// درآمدها
        /// </summary>
        public required IReadOnlyCollection<Item> Revenues { get; init; }
        /// <summary>
        /// هزینه‌ها
        /// </summary>
        public required IReadOnlyCollection<Item> Expenses { get; init; }

        /// <summary>
        /// جمع کل درآمدها
        /// </summary>
        public decimal TotalRevenues => Revenues.Sum(e => e.Balance);
        /// <summary>
        /// جمع کل هزینه‌ها
        /// </summary>
        public decimal TotalExpenses => Expenses.Sum(e => e.Balance);
        /// <summary>
        /// سود/زیان
        /// </summary>
        public decimal Total => TotalRevenues - TotalExpenses;

        public class Item
        {
            public required int AccountId { get; init; }
            public required string AccountCode { get; init; }
            public required string AccountName { get; init; }
            public required decimal Balance { get; init; }
        }
    }
}
