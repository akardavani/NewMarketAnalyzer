using System;

namespace MarketAnalyzer.SharedKernel
{
    public class CompanyBankAccountInfo
    {
        /// <summary>
        /// شناسه 
        /// </summary>
        public required int Id { get; init; }
        /// <summary>
        /// شماره حساب
        /// </summary>
        public required string AccountNumber { get; init; }
        /// <summary>
        /// شماره شبا
        /// </summary>
        public required string ShebaNumber { get; init; }
        /// <summary>
        /// شناسه ملی بانک
        /// </summary>
        public required string BankNationalCode { get; init; }
        /// <summary>
        /// کد شعبه
        /// </summary>
        public required string BankCode { get; init; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public required string BankName { get; init; }
        /// <summary>
        /// کد شعبه
        /// </summary>
        public required string BranchCode { get; init; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public required string BranchName { get; init; }
        /// <summary>
        /// شناسه نوع حساب
        /// </summary>
        public required int AccountTypeId { get; init; }
        /// <summary>
        /// نوع حساب
        /// </summary>
        public required BankAccountTypeKernel AccountType { get; init; }
        /// <summary>
        /// نام نوع حساب
        /// </summary>
        public required string AccountTypeName { get; init; }
        /// <summary>
        /// روز دریافت سود
        /// </summary>
        public required int? GetProfitDateInMonth { get; init; }
        /// <summary>
        /// کد حساب تفصیلی
        /// </summary>
        public required string DetailAccountCode { get; init; }
        /// <summary>
        /// نرخ سود
        /// </summary>
        public required decimal InterestRate { get; init; }
        /// <summary>
        /// تاریخ افتتاح حساب
        /// </summary>
        public required DateOnly OpeningDate { get; init; }
        /// <summary>
        /// فعال/غیرفعال
        /// </summary>
        public required bool IsActive { get; init; }
        /// <summary>
        /// تاریخ سر رسید
        /// </summary>
        public required DateOnly? MatureDate { get; init; }

    }
}
