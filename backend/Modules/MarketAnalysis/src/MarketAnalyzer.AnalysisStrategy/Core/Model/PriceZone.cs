namespace MarketAnalyzer.AnalysisStrategy;

public sealed class PriceZone
{
    public decimal Low { get; set; }

    public decimal High { get; set; }

    public decimal Center => (Low + High) / 2m;
}