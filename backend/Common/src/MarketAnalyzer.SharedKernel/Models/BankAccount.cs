using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;

namespace MarketAnalyzer.SharedKernel
{
    public class BankAccount : ValueObject
    {
        public BankAccount(
            int bankId,
            string bankName,
            int accountTypeId,
            string accountType,
            string branchName,
            string branchCode,
            string accountNumber,
            string shebaNumber)
        {
            if (string.IsNullOrEmpty(accountType))
            {
                throw new ArgumentException($"'{nameof(accountType)}' cannot be null or empty.", nameof(accountType));
            }

            if (string.IsNullOrEmpty(bankName))
            {
                throw new ArgumentException($"'{nameof(bankName)}' cannot be null or empty.", nameof(bankName));
            }

            if (string.IsNullOrEmpty(branchName))
            {
                throw new ArgumentException($"'{nameof(branchName)}' cannot be null or empty.", nameof(branchName));
            }

            if (string.IsNullOrEmpty(branchCode))
            {
                throw new ArgumentException($"'{nameof(branchCode)}' cannot be null or empty.", nameof(branchCode));
            }

            if (string.IsNullOrEmpty(accountNumber))
            {
                throw new ArgumentException($"'{nameof(accountNumber)}' cannot be null or empty.", nameof(accountNumber));
            }

            if (string.IsNullOrEmpty(shebaNumber))
            {
                throw new ArgumentException($"'{nameof(shebaNumber)}' cannot be null or empty.", nameof(shebaNumber));
            }

            BankId = bankId;
            BankName = bankName;
            AccountTypeId = accountTypeId;
            AccountType = accountType;
            BranchName = branchName;
            BranchCode = branchCode;
            AccountNumber = accountNumber;
            ShebaNumber = shebaNumber;
        }

        /// <summary>
        /// شناسه بانک
        /// </summary>
        public int BankId { get; private set; }
        /// <summary>
        /// نام بانک
        /// </summary>
        public string BankName { get; private set; }
        /// <summary>
        /// شناسه نوع حساب
        /// </summary>
        public int AccountTypeId { get; private set; }
        /// <summary>
        /// نوع حساب
        /// </summary>
        public string AccountType { get; private set; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public string BranchName { get; private set; }
        /// <summary>
        /// کد شعبه
        /// </summary>
        public string BranchCode { get; private set; }
        /// <summary>
        /// شماره حساب
        /// </summary>
        public string AccountNumber { get; private set; }
        /// <summary>
        /// شماره شبا
        /// </summary>
        public string ShebaNumber { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return BankId;
            yield return BankName;
            yield return AccountTypeId;
            yield return AccountType;
            yield return BranchName;
            yield return BranchCode;
            yield return AccountNumber;
            yield return ShebaNumber;
        }
    }
}
