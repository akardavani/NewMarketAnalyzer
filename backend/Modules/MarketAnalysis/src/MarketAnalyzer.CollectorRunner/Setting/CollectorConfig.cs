namespace MarketAnalyzer.CollectorRunner
{
    public class CollectorConfig
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public List<TimeframeSchedule> Timeframes { get; set; } = new();
    }

}
