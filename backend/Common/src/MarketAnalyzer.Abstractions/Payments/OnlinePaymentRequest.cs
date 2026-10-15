using System;

namespace MarketAnalyzer.Abstractions
{
    public class OnlinePaymentRequest
    {
        /// <summary>
        /// شناسه داخلی درخواست
        /// </summary>
        public required long Id { get; init; }
        /// <summary>
        /// تاریخ درخواست
        /// </summary>
        public required DateTime RequestDate { get; init; }
        /// <summary>
        /// نام نمایشی در درگاه پرداخت
        /// </summary>
        public required string DisplayName { get; init; }
        /// <summary>
        /// آدرس درگاه پرداخت
        /// </summary>
        public required string GatewayUrl { get; init; }
        /// <summary>
        /// کد ملی واریز کننده
        /// </summary>
        public required string NationalCode { get; init; }
        /// <summary>
        /// مبلغ به ریال
        /// </summary>
        public required decimal Amount { get; init; }
        /// <summary>
        /// JSON تنظیمات درگاه پرداخت در قالب
        /// </summary>
        public required string GatewayConfig { get; init; }
        /// <summary>
        /// آدرس پردازش درخواست پرداخت
        /// </summary>
        public required string CallbackUrl { get; init; }
        /// <summary>
        /// شماره موبایل واریز کننده
        /// </summary>
        public required string MobileNumber { get; init; }
        /// <summary>
        /// شماره کارت
        /// </summary>
        public required string CardNumber { get; init; }
    }
}
