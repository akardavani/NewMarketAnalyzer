namespace MarketAnalyzer.Api.Models
{
    public class CreateNewsRequest
    {
        public string Title { get; set; }
        public string Source { get; set; }
        public string Summary { get; set; }
        public string Text { get; set; }
        public bool IsActive { get; set; }
    }
}
