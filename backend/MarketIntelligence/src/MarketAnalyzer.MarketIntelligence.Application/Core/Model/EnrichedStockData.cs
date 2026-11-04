using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application;

public sealed class EnrichedStockData
{
    public ExcelStockRow ExcelData { get; set; }

    public string Symbol { get; set; }
    public string InstrumentId { get; set; }

    public string IndustryId { get; set; }

    public string IndustryName { get; set; }

    public decimal MarketCap { get; set; }

    public decimal FreeFloatPercent { get; set; }

    public IReadOnlyList<AnalysisCandle> DailyCandles { get; set; }

    //public IReadOnlyList<AnalysisCandle> WeeklyCandles { get; set; }

    public IReadOnlyList<AnalysisCandle> MonthlyCandles { get; set; }

    public IReadOnlyList<AnalysisCandle> IndustryIndexCandles { get; set; }

    public IReadOnlyList<AnalysisCandle> MarketIndexCandles { get; set; }
}