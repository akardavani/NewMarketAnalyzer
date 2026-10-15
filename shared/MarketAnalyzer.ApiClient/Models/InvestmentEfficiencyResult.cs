namespace MarketAnalyzer.ApiClient.Models
{
  public class InvestmentEfficiencyResult
  {
    /// <summary>
    /// ارزش دارایی پس از سرمایه گذاری
    /// </summary>
    public required decimal AssetsValue { get; init; }
    /// <summary>
    /// سود و زیان سرمایه گذاری
    /// </summary>
    public decimal Income { get; init; }
    /// <summary>
    /// درصد سود و زیان
    /// </summary>
    public decimal IncomePercent { get; init; }
  }
}
