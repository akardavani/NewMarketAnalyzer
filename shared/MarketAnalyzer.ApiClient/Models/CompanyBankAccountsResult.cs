namespace MarketAnalyzer.ApiClient.Models
{
  /// <summary>
  /// حساب های بانکی صندوق
  /// </summary>
  public class CompanyBankAccountsResult
  {
    /// <summary>
    /// شناسه حساب بانکی شرکت
    /// </summary>
    public required int Id { get; init; }
    /// <summary>
    /// نام بانک
    /// </summary>
    public required string Bank { get; init; }
    /// <summary>
    /// شماره حساب
    /// </summary>
    public required string AccountNumber { get; init; }
    /// <summary>
    /// شماره شبا
    /// </summary>
    public required string ShebaNumber { get; init; }
    /// <summary>
    /// نام شعبه
    /// </summary>
    public required string BankBranch { get; init; }
    /// <summary>
    /// کد شعبه
    /// </summary>
    public required string BankBranchCode { get; init; }
    /// <summary>
    /// نام نوع حساب
    /// </summary>
    public required string AccountType { get; init; }
  }
}
