namespace MarketAnalyzer.MarketIntelligence.Application
{
    public class SignalResult
    {
        public decimal Entry { get; set; }

        public decimal StopLoss { get; set; }

        public decimal[] TakeProfits { get; set; }
    }
}
