using System;
using System.Collections.Generic;

namespace MarketAnalyzer.Abstractions
{
    public class BatchStatusInquiryResult
    {
        /// <summary>
        /// وضعیت
        /// </summary>
        public BatchTransferState Status { get; init; }
        /// <summary>
        /// تاریخ و ساعت به میلادی
        /// </summary>
        public required DateTime EnteredDate { get; init; }
        /// <summary>
        /// تاریخ و ساعت تایید تراکنش
        /// </summary>
        public required DateTime AuthDate { get; init; }
        /// <summary>
        /// شماره مرجع تراکنش
        /// </summary>
        public required string TransactionId { get; init; }
        /// <summary>
        /// کارمزد تراکنش
        /// </summary>
        public required decimal Commission { get; init; }
        /// <summary>
        /// مبلغ کل تراکنش
        /// </summary>
        public required decimal TotalAmount { get; init; }
        /// <summary>
        /// شرح تراکنش
        /// </summary>
        public required string Description { get; init; }
        /// <summary>
        /// کد خطا
        /// </summary>
        public required string ErrorCode { get; init; }
        /// <summary>
        /// شرح خطا
        /// </summary>
        public required string ErrorTitle { get; init; }
        /// <summary>
        /// درخواستها
        /// </summary>
        public required IReadOnlyCollection<Item> Items { get; init; }

        public class Item
        {
            /// <summary>
            /// وضعیت
            /// </summary>
            public BatchTransferItemState Status { get; init; }
            /// <summary>
            /// شماره حساب مقصد
            /// </summary>
            public string AccountNumber { get; init; }
            /// <summary>
            /// مبلغ تراکنش
            /// </summary>
            public decimal Amount { get; init; }
            /// <summary>
            /// شرح تراکنش
            /// </summary>
            public string Description { get; init; }
            /// <summary>
            /// شماره مرجع
            /// </summary>
            public string ReferenceNumber { get; init; }
            /// <summary>
            /// شناسه پرداخت
            /// </summary>
            public string PayerId { get; init; }
            /// <summary>
            /// تاریخ تراکنش
            /// </summary>
            public DateTime Date { get; init; }
            /// <summary>
            /// دلیل برگشت تراکنش
            /// </summary>
            public string RejectDescription { get; init; }
        }
    }
}