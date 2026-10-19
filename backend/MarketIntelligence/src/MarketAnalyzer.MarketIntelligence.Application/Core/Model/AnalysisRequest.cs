namespace MarketAnalyzer.MarketIntelligence.Application
{
    public class AnalysisRequest
    {
        public string Strategy { get; set; }   
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
}
