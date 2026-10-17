namespace MarketAnalyzer.Abstractions
{
    public class TradingBroker
    {
        /// <summary>
        /// کد شناسه
        /// </summary>
        public required int Id { get; init; }
        /// <summary>
        /// نام کارگزاری
        /// </summary>
        public required string Name { get; init; }
        /// <summary>
        /// کد کارگزاری
        /// </summary>
        public required string NscCode { get; init; }
        /// <summary>
        /// کد حساب تفصیلی
        /// </summary>
        public required string DetailAccountCode { get; init; }
        /// <summary>
        /// شماره مشتری صندوق در کارگزاری
        /// </summary>
        public required string ClientAccountNumber { get; init; }
    }
}
