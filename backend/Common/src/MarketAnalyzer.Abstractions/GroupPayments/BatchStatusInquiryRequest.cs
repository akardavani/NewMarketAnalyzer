namespace MarketAnalyzer.Abstractions
{
    public class BatchStatusInquiryRequest
    {
        /// <summary>
        /// شماره مرجع تراکنش
        /// </summary>
        public required string TransactionId { get; init; }
        /// <summary>
        /// Json پارامترهای درگاره پرداخت در قالب 
        /// </summary>
        public required string Config { get; init; }
    }
}