namespace MarketIntelligence.Application;

/// <summary>
/// =======================================================================
/// REQUIRED INPUT DATA
/// =======================================================================
///
/// Per Stock:
///
/// InstrumentId
/// IndustryId
/// IndustryName
///
/// MarketCap
///
/// ClosingPrice
///
/// EPS
/// PE
/// PB
/// PS
/// DPS
///
/// RSI
/// MFI
///
/// SMA50
/// SMA200
///
/// DemandToTradeRatio
///
/// VolumeToday
/// VolumeAvg30
/// VolumeAvg90
///
/// DailyCandles
/// MonthlyCandles
///
/// -----------------------------------------------------------------------
///
/// Per Industry:
///
/// Industry Index Candles
///
/// -----------------------------------------------------------------------
///
/// Market:
///
/// Market Index Candles
///
/// -----------------------------------------------------------------------
///
/// Optional Future Features:
///
/// FreeFloatPercent
/// OwnershipChanges
/// RealMoneyFlow
/// ShareholderCount
///
/// =======================================================================
/// </summary>
/// 
public sealed class StockAnalysisInput
{
    public EnrichedStockData Data { get; set; }

    public MarketDerivedMetrics Metrics { get; set; }
}
