namespace MarketAnalyzer.ApiClient.Models
{
  public class ClientSelectOption : ClientSelectOption<long>
  {
  }

  public class ClientSelectOption<T>
  {
    public T Value { get; set; }
    public string Text { get; set; }
  }
}
