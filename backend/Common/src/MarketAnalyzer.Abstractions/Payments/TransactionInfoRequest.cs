namespace MarketAnalyzer.Abstractions
{
    /// <summary>
    /// درخواست مشخصات تراکنش بانکی
    /// </summary>
    public class TransactionInfoRequest
    {
        /// <summary>
        /// شناسه داخلی سیستم
        /// </summary>
        public required long InsternalRequestId { get; init; }
        /// <summary>
        /// شناسه تراکنش که بعد از پرداخت بانک به ما میدهد، با کمک این پارامتر و شناسه میتوانیم صحت تراکنش را بررسی کنیم
        /// </summary>
        public required string TransactionId { get; init; }
    }
}
