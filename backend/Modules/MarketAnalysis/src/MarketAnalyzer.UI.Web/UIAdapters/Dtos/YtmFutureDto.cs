namespace MarketAnalyzer.UI.Web
{
    public class YtmFutureDto
    {
        public string BaseSymbol { get; set; } = string.Empty;
        public string CompareSymbol { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<YtmFuturePointDto> Points { get; set; } = new();
    }
}
