using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace MarketAnalyzer.Abstractions
{
    public class ProcessTransactionRequest
    {
        /// <summary>
        /// شناسه درخواست در سیستم داخلی
        /// </summary>
        public required long InternalRequestId { get; init; }
        /// <summary>
        /// مبلغ تراکنش
        /// </summary>
        public required decimal Amount { get; init; }
        /// <summary>
        /// شناسه درخواست پرداخت که از طرف بانک به ما داده میشود، در مرحله اول برای درخواست پرداخت صادر میشود
        /// </summary>
        public required string RequestId { get; init; }
        /// <summary>
        /// JSON تنظیمات درگاه پرداخت در قالب 
        /// </summary>
        public required string GatewayConfig { get; init; }
        /// <summary>
        /// داده‌های فرم ارسال شده توسط درگاه بانک
        /// </summary>
        public required IReadOnlyDictionary<string, StringValues> FormData { get; init; }        
    }     
}
