namespace MarketAnalyzer.UI.Web
{
    public class YtmFuturePointDto
    {
        public DateOnly Date { get; set; }
        public string PersianDate { get; set; } = string.Empty;
        public decimal Growth { get; set; }
    }

}
