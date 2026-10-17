using System.Collections.Generic;

namespace MarketAnalyzer.Abstractions
{
    public class BatchTransferRequest
    {
        /// <summary>
        /// Json پارامترهای درگاره پرداخت در قالب 
        /// </summary>
        public required string Config { get; init; }
        /// <summary>
        /// شماره حساب مبدأ
        /// </summary>
        public required string AccountNumber { get; init; }
        /// <summary>
        /// شرح تراکنش - حداکثر 50 کارکتر
        /// </summary>
        public required string Description { get; init; }
        /// <summary>
        /// بابت
        /// </summary>
        public required string Reason { get; init; }
        /// <summary>
        /// شماره مرجع - توسط مشتری ایجاد میشود
        /// </summary>
        public required string CustomerRef { get; init; }
        /// <summary>
        /// لیست مقاصد واریز
        /// </summary>
        public required IReadOnlyCollection<Item> Items { get; init; }

        public class Item
        {
            /// <summary>
            /// شماره مرجع درخواست
            /// </summary>
            public required string RequestId { get; init; }
            /// <summary>
            /// شماره حساب مقصد
            /// </summary>
            public required string AccountNumber { get; init; }
            /// <summary>
            /// مبلغ
            /// </summary>
            public required decimal Amount { get; init; }
            /// <summary>
            /// نام صاحب حساب
            /// </summary>
            public required string HolderName { get; init; }
            /// <summary>
            /// شناسه پرداخت کننده
            /// </summary>
            public required string PayerId { get; init; }
            /// <summary>
            /// توضیحات - حداکثر 75 کارکتر
            /// </summary>
            public required string Description { get; init; }
        }
    }
}