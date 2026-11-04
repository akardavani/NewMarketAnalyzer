using MarketAnalyzer.MarketData.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.Core.Model;

/// <summary>A normalized candle value used by analysis strategies and indicators.</summary>
public sealed class AnalysisCandle
{
    public required string Id { get; init; }
    public required string InstrumentId { get; init; }
    public required string MarketKey { get; init; }
    public required Timeframe Timeframe { get; init; }
    public decimal Open { get; init; }
    public decimal Close { get; init; }
    public decimal High { get; init; }
    public decimal Low { get; init; }
    public decimal Volume { get; init; }
    public DateTime Date { get; init; }
    public decimal NetValue { get; init; }
    public long Timestamp { get; init; }
    public required string PersianDate { get; init; }

    public decimal Range => High - Low;
    public decimal Body => Math.Abs(Open - Close);
    public bool IsBullish => Close > Open;
    public bool IsBearish => Close < Open;
    public decimal UpperWick => High - Math.Max(Close, Open);
    public decimal LowerWick => Math.Min(Close, Open) - Low;
    public bool Rally => IsBullish && Body > UpperWick + LowerWick;
    public bool Drop => IsBearish && Body > UpperWick + LowerWick;
    public bool Base => Body < UpperWick + LowerWick;
    public double BodyPct => (double)Body / (double)Range;
    public double UpperWickPct => (double)UpperWick / (double)Range;
    public double LowerWickPct => (double)LowerWick / (double)Range;
}
