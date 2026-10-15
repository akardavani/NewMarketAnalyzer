namespace MarketAnalyzer.ApiClient.Models
{
  public class FormFile
  {
    /// <summary>
    /// حجم فایل
    /// </summary>
    public long Size { get; set; }
    /// <summary>
    /// نام فایل
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// نوع فایل
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// محتوای فایل
    /// </summary>
    public string Content { get; set; }
  }
}
