namespace MarketAnalyzer.Abstractions
{
    public class ProcessRequestOptions
    {
        /// <summary>
        /// آدرس درگاه پرداخت
        /// </summary>
        public required string GatewayUrl { get; init; }
        /// <summary>
        /// شناسه درخواست پرداخت که از طرف بانک به ما داده میشود، در مرحله اول برای درخواست پرداخت صادر میشود
        /// </summary>
        public required string RequestId { get; init; }
    }
}
