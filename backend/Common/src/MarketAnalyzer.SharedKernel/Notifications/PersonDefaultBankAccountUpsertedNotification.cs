using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.SharedKernel
{
    public class PersonDefaultBankAccountUpsertedNotification : INotification
    {
        /// <summary>
        /// شناسه شخص
        /// </summary>
        public required long PersonId { get; init; }
        /// <summary>
        /// شناسه بانک
        /// </summary>
        public required int BankId { get; init; }
        /// <summary>
        /// نام بانک
        /// </summary>
        public required string BankName { get; init; }
        /// <summary>
        /// شناسه نوع حساب
        /// </summary>
        public required int AccountTypeId { get; init; }
        /// <summary>
        /// نوع حساب
        /// </summary>
        public required string AccountType { get; init; }
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
        public required string Number { get; init; }
        /// <summary>
        /// شماره شبا
        /// </summary>
        public required string ShebaNumber { get; init; }
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public required int OperatorId { get; init; }
    }
}
