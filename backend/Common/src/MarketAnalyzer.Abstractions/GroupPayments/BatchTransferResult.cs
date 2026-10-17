using System.Collections.Generic;

namespace MarketAnalyzer.Abstractions
{
    public class BatchTransferResult
    {
        /// <summary>
        /// وضعیت
        /// </summary>
        public BatchTransferState State { get; init; }
        /// <summary>
        /// کد خطا
        /// </summary>
        public required string ErrorCode { get; init; }
        /// <summary>
        /// شرح خطا
        /// </summary>
        public required string ErrorTitle { get; init; }
        /// <summary>
        /// شماره مرجع تراکنش
        /// </summary>
        public required string TransactionId { get; init; }
        /// <summary>
        /// لیست تراکنش هایی که دلیل وجود مشکل انجام نشدهاند
        /// </summary>
        public required IReadOnlyCollection<RejectedItem> Rejected { get; init; }

        public class RejectedItem
        {
            /// <summary>
            /// مبلغ
            /// </summary>
            public required string Amount { get; init; }
            /// <summary>
            /// شرح خطا
            /// </summary>
            public required string Error { get; init; }
            /// <summary>
            /// شناسه پرداخت
            /// </summary>
            public required string PayerId { get; init; }
            /// <summary>
            /// شماره حساب مقصد
            /// </summary>
            public required string AccountNumber { get; init; }
            /// <summary>
            /// شناسه درخواست
            /// </summary>
            public required string RequestId { get; init; }
        }
    }
}