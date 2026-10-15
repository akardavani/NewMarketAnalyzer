namespace MarketAnalyzer.Abstractions
{
    /// <summary>
    /// اطلاعات تراکنش بانکی
    /// </summary>
    public class TransactionInfo
    {
        /// <summary>
        /// قابل تایید
        /// </summary>
        public required bool Verifiable { get; init; }
        /// <summary>
        /// کد ملی سرمایه گذار
        /// </summary>
        public required string InvestorNationalCode { get; init; }
        /// <summary>
        /// شناسه داخلی سیستم
        /// </summary>
        public required long InsternalRequestId { get; init; }
        /// <summary>
        /// مبلغ تراکنش
        /// </summary>
        public required decimal Amount { get; init; }
        /// <summary>
        /// شناسه درخواست پرداخت که از طرف بانک به ما داده میشود، در مرحله اول برای درخواست پرداخت صادر میشود
        /// </summary>
        public required string RequestId { get; init; }
        /// <summary>
        /// شناسه تراکنش که بعد از پرداخت بانک به ما میدهد، با کمک این پارامتر و شناسه میتوانیم صحت تراکنش را بررسی کنیم
        /// </summary>
        public required string TransactionId { get; init; }
        /// <summary>
        /// JSON تنظیمات درگاه پرداخت در قالب 
        /// </summary>
        public required string GatewayConfig { get; init; }
        /// <summary>
        /// JSON دیتای اصلی تراکنش در قالب
        /// </summary>
        public required string TransactionData { get; init; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public required string State { get; init; }
    }
}
