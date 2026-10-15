namespace MarketAnalyzer.SharedKernel
{
    public class VoucherTypeInfo
    {
        /// <summary>
        /// شناسه نوع سند
        /// </summary>
        public required int Id { get; init; }
        /// <summary>
        /// نام نوع سند
        /// </summary>
        public required string Name { get; init; }
    }
}
