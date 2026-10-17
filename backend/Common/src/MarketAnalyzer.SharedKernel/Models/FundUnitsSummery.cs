namespace MarketAnalyzer.SharedKernel
{
    public class FundUnitsSummery
    {
        /// <summary>
        /// ارزش مبنا
        /// </summary>
        public required decimal NominalValue { get; init; }
        /// <summary>
        /// تعداد واحدها
        /// </summary>
        public required long UnitCounts { get; init; }
    }
}
