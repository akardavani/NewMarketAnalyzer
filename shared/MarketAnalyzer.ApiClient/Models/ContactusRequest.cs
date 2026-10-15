namespace MarketAnalyzer.ApiClient.Models
{
  public class ContactusRequest
  {
    /// <summary>
    /// نام و نام خانوادگی
    /// </summary>
    public string FullName { get; set; }
    /// <summary>
    /// ایمیل
    /// </summary>
    public string Email { get; set; }
    /// <summary>
    /// شماره تماس
    /// </summary>
    public string PhoneNumber { get; set; }
    /// <summary>
    /// موضوع
    /// </summary>
    public string Subject { get; set; }
    /// <summary>
    /// متن پیام
    /// </summary>
    public string Text { get; set; }
    /// <summary>
    /// آدرس IP کاربر
    /// </summary>
    public string IpAddress { get; set; }
  }
}
