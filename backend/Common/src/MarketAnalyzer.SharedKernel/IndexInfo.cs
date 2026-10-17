namespace MarketAnalyzer.SharedKernel
{
    /// <summary>
    /// اطلاعات شاخص کل
    /// </summary>
    public class IndexInfo
    {
        /// <summary>
        /// مقدار شاخص یکروز قبل از تاریخ مورد نظر
        /// </summary>
        public required decimal BeforeDateIndexValue { get; init; }
        /// <summary>
        /// مقدار شاخص در تاریخ مورد نظر
        /// </summary>
        public required decimal DateIndexValue { get; init; }
        /// <summary>
        /// مقدار شاخص در تاریخ پایه
        /// </summary>
        public required decimal BaseDateIndexValue { get; init; }
    }
}
