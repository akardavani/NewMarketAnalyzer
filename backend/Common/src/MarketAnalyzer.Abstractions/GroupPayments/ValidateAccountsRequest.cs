using System.Collections.Generic;

namespace MarketAnalyzer.Abstractions
{
    public class ValidateAccountsRequest
    {
        /// <summary>
        /// Json پارامترهای درگاره پرداخت در قالب 
        /// </summary>
        public required string Config { get; init; }
        /// <summary>
        /// حساب‌هایی که باید اعتبارسنجی شوند
        /// </summary>
        public required IReadOnlyCollection<Account> Accounts { get; init; }

        public class Account
        {
            /// <summary>
            /// شماره حساب
            /// </summary>
            public required string AccountNumber { get; init; }
        }
    }
}