namespace MarketAnalyzer.SharedKernel
{
    public class FinancialStatement
    {
        /// <summary>
        /// دارایی‌ها
        /// </summary>
        public required decimal TotalAssets { get; init; }
        /// <summary>
        /// سرمایه گذاری‌ها
        /// </summary>
        public required decimal TotalInvestments { get; init; }
        /// <summary>
        /// بدهی‌ها
        /// </summary>
        public required decimal TotalLiabilities { get; init; }       
        /// <summary>
        /// درآمدها
        /// </summary>
        public required decimal TotalRevenues { get; init; }
        /// <summary>
        /// هزینه‌ها
        /// </summary>
        public required decimal TotalExpenses { get; init; }

        /// <summary>
        /// سود/زیان
        /// </summary>
        public decimal TotalIncome => TotalRevenues - TotalExpenses;
        /// <summary>
        /// خالص ارزش دارایی‌ها
        /// </summary>
        public decimal NetAssetsValue => TotalInvestments + TotalAssets - TotalLiabilities;
    }
}
