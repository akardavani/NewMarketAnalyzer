namespace MarketAnalyzer.SharedKernel
{
    public class IndexPerformanceResult
    {
        /// <summary>
        /// مقدار شاخص
        /// </summary>
        public required decimal IndexValue { get; init; }
        /// <summary>
        /// مقدار شاخص پایه
        /// </summary>
        public required decimal BaseIndexValue { get; init; }
        /// <summary>
        /// مقدار بازدهی شخص
        /// </summary>
        public required decimal PerformanceValue { get; init; }
    }
}
