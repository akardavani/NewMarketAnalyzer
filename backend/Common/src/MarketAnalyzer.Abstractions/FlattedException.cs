namespace MarketAnalyzer.Abstractions
{
    public class FlattedException
    {
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public FlattedException InnerException { get; set; }
    }
}
