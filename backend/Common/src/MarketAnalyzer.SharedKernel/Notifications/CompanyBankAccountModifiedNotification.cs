using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.SharedKernel
{
    public class CompanyBankAccountModifiedNotification : INotification
    {
        /// <summary>
        /// شناسه حساب بانکی
        /// </summary>
        public required int Id { get; init; }
        /// <summary>
        /// نام بانک
        /// </summary>
        public required string BankName { get; init; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public required string BranchName { get; init; }
        /// <summary>
        /// کد شعبه
        /// </summary>
        public required string BranchCode { get; init; }
        /// <summary>
        /// شماره حساب
        /// </summary>
        public required string AccountNumber { get; init; }
        /// <summary>
        /// شماره شبا
        /// </summary>
        public required string ShebaNumber { get; init; }
        /// <summary>
        /// نوع حساب
        /// </summary>
        public required string AccountType { get; init; }
        /// <summary>
        /// دسته بندی
        /// </summary>
        public required BankAccountCategory Category { get; init; }
        /// <summary>
        /// روز دریافت سود 
        /// </summary>
        public required int InterestDueDate { get; init; }
        /// <summary>
        /// نرخ سود
        /// </summary>
        public required decimal InterestRate { get; init; }
        /// <summary>
        /// سود تنزیل سود دارد/ندارد
        /// </summary>
        public required bool HasDiscounting { get; init; }
        /// <summary>
        /// تاریخ افتتاح حساب
        /// </summary>
        public required DateOnly OpeningDate { get; init; }
        /// <summary>
        /// مبلغ سرمایه گذاری/افتتاح حساب
        /// </summary>
        public required decimal OpeningAmount { get; init; }
        /// <summary>
        /// درصد کارمزد مدیر
        /// </summary>
        public required decimal ManagerCommissionFee { get; init; }
        /// <summary>
        /// در نظر گرفتن کارمزد برای مدیر
        /// </summary>
        public required bool ApplyManagerCommission { get; init; }
        /// <summary>
        /// تاریخ سر رسید
        /// </summary>
        public required DateOnly? MatureDate { get; init; }
        /// <summary>
        /// فعال
        /// </summary>
        public required bool IsActive { get; init; }
        /// <summary>
        /// شناسه کاربر ایجاد کننده
        /// </summary>
        public required int OperatorId { get; init; }
    }
}