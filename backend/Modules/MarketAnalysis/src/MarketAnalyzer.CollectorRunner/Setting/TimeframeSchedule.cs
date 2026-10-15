namespace MarketAnalyzer.CollectorRunner
{
    public class TimeframeSchedule
    {
        public string Timeframe { get; set; }
        public string Cron { get; set; }
        public List<string> Methods { get; set; } = new();
    }

}
