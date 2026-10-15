using MarketAnalyzer.AnalysisStrategy;

namespace MarketAnalyzer.UI.Web
{
    public class IranEquityMainDto
    {
        public DateTime MarketTimestamp { get; set; }

        public int TotalSymbolsAnalyzed { get; set; }

        public List<IranEquityStrategyResult> TopPicks { get; set; }

        public List<SectorAnalysisResult> LeadingSectors { get; set; }

        public List<SectorAnalysisResult> FullSectorReport { get; set; }
    }
}
