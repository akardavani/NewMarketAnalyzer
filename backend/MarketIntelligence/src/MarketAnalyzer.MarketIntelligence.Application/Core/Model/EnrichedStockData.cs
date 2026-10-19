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

    public IReadOnlyList<MarketCandle> DailyCandles { get; set; }

    //public IReadOnlyList<MarketCandle> WeeklyCandles { get; set; }

    public IReadOnlyList<MarketCandle> MonthlyCandles { get; set; }

    public IReadOnlyList<MarketCandle> IndustryIndexCandles { get; set; }

    public IReadOnlyList<MarketCandle> MarketIndexCandles { get; set; }
}