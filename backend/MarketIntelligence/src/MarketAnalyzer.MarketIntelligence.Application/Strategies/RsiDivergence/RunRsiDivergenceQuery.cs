namespace MarketAnalyzer.MarketIntelligence.Application.Strategies.RsiDivergence;

public sealed record RunRsiDivergenceQuery
{
    public bool IncludeSuspects { get; init; } = true;
    public bool IncludeBacktest { get; init; } = true;
}

public sealed class RunRsiDivergenceResult
{
    public DateTime GeneratedAt { get; init; }
    public int TotalSymbols { get; init; }
    public int AnalyzedSymbols { get; init; }
    public int MissingHistorySymbols { get; init; }
    public int StaleSymbols { get; init; }
    public int IlliquidSymbols { get; init; }
    public IReadOnlyList<RsiDivergenceSignal> Signals { get; init; } = Array.Empty<RsiDivergenceSignal>();
    public IReadOnlyList<RsiDivergenceSuspect> Suspects { get; init; } = Array.Empty<RsiDivergenceSuspect>();
    public IReadOnlyList<RsiDivergenceBacktestSummary> Backtest { get; init; } = Array.Empty<RsiDivergenceBacktestSummary>();
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();
}

public sealed record RsiDivergenceSignal(
    string Symbol,
    string SymbolKey,
    string Timeframe,
    string Status,
    DateTime FirstLowDate,
    DateTime SecondLowDate,
    DateTime SignalDate,
    decimal FirstLow,
    decimal SecondLow,
    double FirstRsi,
    double SecondRsi,
    decimal? EntryPrice,
    decimal? StopPrice,
    decimal? TargetOne,
    decimal? TargetTwo,
    decimal LastPrice,
    decimal? CurrentReturnPercent);

public sealed record RsiDivergenceSuspect(
    string Symbol,
    string SymbolKey,
    string Timeframe,
    string Kind,
    DateTime FirstLowDate,
    DateTime CandidateLowDate,
    decimal FirstLow,
    decimal CandidateLow,
    double FirstRsi,
    double CandidateRsi,
    int BarsUntilPivotConfirmation);

public sealed record RsiDivergenceBacktestSummary(
    string Timeframe,
    int Trades,
    decimal WinRatePercent,
    decimal AverageReturnPercent,
    decimal MedianReturnPercent,
    decimal ProfitFactor,
    decimal TargetOneHitPercent,
    decimal TargetTwoHitPercent,
    decimal StopHitPercent,
    decimal AverageBarsHeld);
