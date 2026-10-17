namespace MarketAnalyzer.Api.Models
{
    public class UpdateNewsRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Source { get; set; }
        public string Summary { get; set; }
        public string Text { get; set; }
    }
}
