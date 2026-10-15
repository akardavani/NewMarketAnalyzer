namespace MarketAnalyzer.SharedKernel
{
    public class BrokerInfo
    {
        /// <summary>
        /// شناسه حساب
        /// </summary>
        public required int Id { get; init; }
        /// <summary>
        /// شماره‌ مشتری صندوق در کارگزاری
        /// </summary>
        public required string ClientAccountNumber { get; init; }
        /// <summary>
        /// نام کارگزاری
        /// </summary>
        public required string Name { get; init; }
    }
}
