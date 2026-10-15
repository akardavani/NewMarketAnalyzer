namespace MarketAnalyzer.ApiClient.Models
{
  public class NewsListRequest
  {
    /// <summary>
    /// شماره صفحه
    /// </summary>
    public int Page { get; set; }
    /// <summary>
    /// کلمه کلیدی جستجو
    /// </summary>
    public string Keyword { get; set; }
  }
}
