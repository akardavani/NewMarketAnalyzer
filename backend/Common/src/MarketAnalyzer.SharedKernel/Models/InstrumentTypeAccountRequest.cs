namespace MarketAnalyzer.SharedKernel
{
    public class InstrumentTypeAccountRequest
    {
        /// <summary>
        /// شناسه نوع نماد
        /// </summary>
        public required int InstrumentTypeId { get; init; }
        /// <summary>
        /// شاخص حساب معین مورد نظر
        /// </summary>
        public required AccountIndicator Indicator { get; init; }
    }
}
