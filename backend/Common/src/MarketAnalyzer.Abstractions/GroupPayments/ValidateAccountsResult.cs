using System.Collections.Generic;

namespace MarketAnalyzer.Abstractions
{
    public class ValidateAccountsResult
    {
        /// <summary>
        /// وضعیت
        /// </summary>
        public required bool Succeeded { get; init; }
        /// <summary>
        /// کد خطا
        /// </summary>
        public required string ErrorCode { get; set; }
        /// <summary>
        /// شرح خطا
        /// </summary>
        public required string ErrorDescription { get; set; }
        /// <summary>
        /// حساب‌ها
        /// </summary>
        public required IReadOnlyCollection<Account> Accounts { get; init; }

        public class Account
        {
            /// <summary>
            /// وضعیت
            /// </summary>
            public required AccountStatus Status { get; init; }
            /// <summary>
            /// شماره حساب وارد شده برای استعلام
            /// </summary>
            public required string InquiryAccountNumber { get; init; }
            /// <summary>
            /// شماره حساب متصل به شماره کارت/شبا/حساب
            /// </summary>
            public required string AccountNumber { get; init; }
            /// <summary>
            /// نام بانک
            /// </summary>
            public required string BankName { get; init; }
            /// <summary>
            /// نام و نام خانوادگی صاحب حساب
            /// </summary>
            public required string HolderName { get; init; }
            /// <summary>
            /// توضیحات
            /// </summary>
            public required string Description { get; init; }
        }
    }
}
