namespace MarketAnalyzer.ApiClient.Models
{
  public abstract class Pagination 
  {
    public int Page { get; set; } = 1;
    public int Rows { get; set; } = 100;
  }
}

