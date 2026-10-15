namespace MarketAnalyzer.ApiClient.Models
{
  public class PillarInformation
  {
    /// <summary>
    /// عنوان رکن
    /// </summary>
    public required string Title { get; init; }
    /// <summary>
    /// توضیحات
    /// </summary>
    public required string Description { get; init; }
    /// <summary>
    /// آدرس فایل رزومه
    /// </summary>
    public required string ResumeUrl { get; init; }
  }
}
