using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.SharedKernel
{
    public class CandleRequestDto
    {
        public string Symbol { get; set; }
        public int YearsAgo { get; set; }
        public Timeframe Timeframe { get; set; }
        public MarketType MarketType { get; set; }
    }
} 
