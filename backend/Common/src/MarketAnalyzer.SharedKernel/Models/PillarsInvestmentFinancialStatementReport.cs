namespace MarketAnalyzer.SharedKernel
{
    public class PillarsInvestmentFinancialStatementReport
    {
        /// <summary>
        /// شناسه
        /// </summary>
        public required string Id { get; init; }
        /// <summary>
        /// نقش
        /// </summary>
        public required string Role { get; init; }
        /// <summary>
        /// نام
        /// </summary>
        public required string FullName { get; init; }
        /// <summary>
        /// نوع واحد
        /// </summary>
        public required string UnitType { get; init; }
        /// <summary>
        /// تعداد واحد
        /// </summary>
        public required long UnitQuantity {get; init; }
        /// <summary>
        /// درصد واحد
        /// </summary>
        public required decimal UnitPercentage { get; init; }
    }
}
