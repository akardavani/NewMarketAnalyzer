using System;

namespace MarketAnalyzer.ApiClient.Models
{
  public class PeriodicAssetCompositionResult
  {
    /// <summary>
    /// میانگین روزانه از ابتدای تأسیس صندوق
    /// </summary>
    public AssetsComposition DailyAverageSinceEstablishment { get; set; }
    /// <summary>
    /// میانگین روزانه دوره ۹۰ روز گذشته
    /// </summary>
    public AssetsComposition DailyAverageNinetyDays { get; set; }
    /// <summary>
    /// میانگین روزانه ۳۰ روز گذشته
    /// </summary>
    public AssetsComposition DailyAverageThreetyDays { get; set; }
    /// <summary>
    /// امروز
    /// </summary>
    public AssetsComposition Today { get; set; }
  }

  public class AssetsComposition
  {
    /// <summary>
    /// تاریخ
    /// </summary>
    public DateOnly Date { get; set; }
    /// <summary>
    /// پنج سهم برتر
    /// </summary>
    public decimal TopFiveStocks { get; set; }
    /// <summary>
    /// سایر سهام
    /// </summary>
    public decimal OtherStocks { get; set; }
    /// <summary>
    /// اوراق بهادار با درآمد ثابت
    /// </summary>
    public decimal Bonds { get; set; }
    /// <summary>
    /// نقد و بانک
    /// </summary>
    public decimal BankAndCach { get; set; }
    /// <summary>
    /// حسابهای دریافتنی تجاری
    /// </summary>
    public decimal CommercialAccountsReceivable { get; set; }
    /// <summary>
    /// سایر دارایی ها
    /// </summary>
    public decimal OtherAssets { get; set; }
    /// <summary>
    /// صندوق سرمایه‌گذاری
    /// </summary>
    public decimal FundInvestment { get; set; }
    /// <summary>
    /// اوراق سپرده بانکی و گواهی سپرده بانکی
    /// </summary>
    public decimal DepositCertificates { get; set; }
    /// <summary>
    /// جاری کارگزاران
    /// </summary>
    public decimal Brokers { get; set; }
    /// <summary>
    /// جمع کل
    /// </summary>
    public decimal Total { get; set; }
    /// <summary>
    /// درصد 5 سهم برتر
    /// </summary>
    public decimal TopFiveStocksPercent { get; set; }
    /// <summary>
    /// درصد سایر سهام
    /// </summary>
    public decimal OtherStocksPercent { get; set; }
    /// <summary>
    /// درصد اوراق با درآمد ثابت
    /// </summary>
    public decimal BondsPercent { get; set; }
    /// <summary>
    /// درصد نقد و بانک
    /// </summary>
    public decimal BankAndCachPercent { get; set; }
    /// <summary>
    /// درصد حسابهای دریافتنی تجاری
    /// </summary>
    public decimal CommercialAccountsReceivablePercent { get; set; }
    /// <summary>
    /// درصد سایر دارایی‌ها
    /// </summary>
    public decimal OtherAssetsPercent { get; set; }
    /// <summary>
    /// درصد حساب فی مابین با کارگزاری
    /// </summary>
    public decimal BrokersPercent { get; set; }
  }
}
