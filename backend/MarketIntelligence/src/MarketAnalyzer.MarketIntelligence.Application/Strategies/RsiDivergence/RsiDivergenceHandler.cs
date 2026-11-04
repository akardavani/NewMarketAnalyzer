using MarketAnalyzer.MarketData.Application.Reading;
using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketIntelligence.Application.Core.Model;
using MarketAnalyzer.MarketIntelligence.Application.Ports;

namespace MarketAnalyzer.MarketIntelligence.Application.Strategies.RsiDivergence;

/// <summary>Runs daily and daily-derived weekly positive RSI divergence scans for the tracked TSE stock universe.</summary>
public sealed class RsiDivergenceHandler
{
    private const double RsiLevel = 30d;
    private const double SuspectRsiLevel = 35d;
    private const decimal MinimumAverageValue = 2_000_000_000m;
    private const decimal StopLoss = 0.10m;
    private const decimal TargetOneRate = 0.15m;
    private const decimal TargetTwoRate = 0.20m;
    private const decimal RoundTripCost = 0.006m;

    private readonly IAnalysisCandleReader _candles;
    private readonly MarketDataIranEquitySymbolCatalog _symbols;

    public RsiDivergenceHandler(IAnalysisCandleReader candles, MarketDataIranEquitySymbolCatalog symbols)
    {
        _candles = candles;
        _symbols = symbols;
    }

    public async Task<RunRsiDivergenceResult> HandleAsync(RunRsiDivergenceQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var universe = await _symbols.GetActiveTseStocksAsync(cancellationToken);
        if (universe.Count == 0)
            return new RunRsiDivergenceResult { GeneratedAt = DateTime.UtcNow };

        var candles = await _candles.GetCandlesAsync(new MultiSymbolCandleQuery
        {
            Markets = new[] { "tse" },
            SymbolKeys = universe.Select(x => x.SymbolKey).ToArray(),
            Timeframe = Timeframe.Daily
        }, cancellationToken);

        var candlesBySymbol = candles
            .GroupBy(x => x.InstrumentId, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.OrderBy(c => c.Date).ToList(), StringComparer.OrdinalIgnoreCase);
        var nameByKey = universe.ToDictionary(x => x.SymbolKey, x => x.PersianName ?? x.Name ?? x.SymbolKey, StringComparer.OrdinalIgnoreCase);
        var signals = new List<RsiDivergenceSignal>();
        var suspects = new List<RsiDivergenceSuspect>();
        var trades = new List<Trade>();
        var errors = new List<string>();
        var analyzed = 0;
        var noHistory = 0;
        var stale = 0;
        var illiquid = 0;
        var today = DateTime.Today;

        foreach (var symbol in universe)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
            if (!candlesBySymbol.TryGetValue(symbol.SymbolKey, out var source) || source.Count == 0)
            {
                noHistory++;
                continue;
            }

            var daily = source.Select(ToBar).ToList();
            var lastDate = daily[^1].Date.Date;
            if ((today - lastDate).TotalDays > 7)
            {
                stale++;
                continue;
            }
            var knownValues = daily.TakeLast(20).Where(x => x.Value.HasValue).Select(x => x.Value!.Value).ToArray();
            if (knownValues.Length > 0 && knownValues.Average() < MinimumAverageValue)
            {
                illiquid++;
                continue;
            }
            analyzed++;

            foreach (var timeframe in new[] { "روزانه", "هفتگی" })
            {
                var bars = timeframe == "روزانه" ? daily : ToWeekly(daily, today);
                var settings = timeframe == "روزانه" ? Parameters.Daily : Parameters.Weekly;
                if (bars.Count < Math.Max(settings.MinimumBars, settings.RsiLength * 3)) continue;

                var rsi = CalculateRsi(bars, settings.RsiLength);
                var pivots = FindPivotLows(bars, settings.Left, settings.Right)
                    .Where(i => rsi[i].HasValue)
                    .ToArray();
                var divergences = FindDivergences(bars, rsi, pivots, settings, RsiLevel);

                if (request.IncludeBacktest)
                    trades.AddRange(Backtest(bars, divergences, settings).Select(t => t with { Timeframe = timeframe }));

                foreach (var divergence in divergences.Where(x => x.SignalIndex >= bars.Count - settings.RecentBars))
                {
                    var confirmation = FindConfirmation(bars, divergence, settings.ConfirmWindow);
                    if (confirmation.Status is ConfirmationStatus.Invalid or ConfirmationStatus.Expired) continue;

                    decimal? entry = null;
                    decimal? stopPrice = null;
                    decimal? targetOne = null;
                    decimal? targetTwo = null;
                    decimal? pnl = null;
                    if (confirmation.Status == ConfirmationStatus.Confirmed)
                    {
                        var isLastConfirmation = confirmation.Index == bars.Count - 1;
                        var entryIndex = isLastConfirmation ? confirmation.Index : confirmation.Index + 1;
                        var entryBar = bars[entryIndex];
                        entry = isLastConfirmation ? entryBar.Close : entryBar.Open > 0 ? entryBar.Open : entryBar.Close;
                        stopPrice = entry * (1m - StopLoss);
                        targetOne = entry * (1m + TargetOneRate);
                        targetTwo = entry * (1m + TargetTwoRate);
                        pnl = isLastConfirmation || entry is null or 0 ? null : decimal.Round((bars[^1].Close / entry.Value - 1m) * 100m, 2);
                    }

                    signals.Add(new RsiDivergenceSignal(
                        nameByKey[symbol.SymbolKey], symbol.SymbolKey, timeframe,
                        confirmation.Status == ConfirmationStatus.Pending ? "در انتظار کندل تأیید" : "تأیید شد",
                        bars[divergence.FirstPivot].Date, bars[divergence.SecondPivot].Date,
                        bars[divergence.SignalIndex].Date, bars[divergence.FirstPivot].Low,
                        bars[divergence.SecondPivot].Low, Round(rsi[divergence.FirstPivot]),
                        Round(rsi[divergence.SecondPivot]), entry, stopPrice, targetOne, targetTwo,
                        bars[^1].Close, pnl));
                }

                if (request.IncludeSuspects)
                {
                    suspects.AddRange(FindDeveloping(bars, rsi, pivots, settings, nameByKey[symbol.SymbolKey], symbol.SymbolKey));
                    suspects.AddRange(FindDivergences(bars, rsi, pivots, settings, SuspectRsiLevel)
                        .Where(x => x.SignalIndex >= bars.Count - settings.RecentBars
                            && (rsi[x.FirstPivot] >= RsiLevel || rsi[x.SecondPivot] >= RsiLevel))
                        .Select(x => new RsiDivergenceSuspect(nameByKey[symbol.SymbolKey], symbol.SymbolKey, timeframe,
                            "واگرایی کامل با RSI بالاتر از آستانه", bars[x.FirstPivot].Date, bars[x.SecondPivot].Date,
                            bars[x.FirstPivot].Low, bars[x.SecondPivot].Low, Round(rsi[x.FirstPivot]), Round(rsi[x.SecondPivot]), 0)));
                }
            }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
            catch (Exception ex) { errors.Add($"{symbol.SymbolKey}: {ex.Message}"); }
        }

        return new RunRsiDivergenceResult
        {
            GeneratedAt = DateTime.UtcNow,
            TotalSymbols = universe.Count,
            AnalyzedSymbols = analyzed,
            MissingHistorySymbols = noHistory,
            StaleSymbols = stale,
            IlliquidSymbols = illiquid,
            Signals = signals.OrderByDescending(x => x.SignalDate).ThenBy(x => x.Symbol, StringComparer.Ordinal).ToArray(),
            Suspects = suspects.OrderByDescending(x => x.CandidateLowDate).ThenBy(x => x.Symbol, StringComparer.Ordinal).ToArray(),
            Backtest = request.IncludeBacktest ? Summarize(trades) : Array.Empty<RsiDivergenceBacktestSummary>(),
            Errors = errors
        };
    }

    private static Bar ToBar(AnalysisCandle candle) => new(candle.Date.Date, candle.Open, candle.High, candle.Low, candle.Close, candle.Volume, candle.NetValue > 0 ? candle.NetValue : null);

    private static List<Bar> ToWeekly(List<Bar> daily, DateTime today)
    {
        var weekly = daily.GroupBy(bar => bar.Date.AddDays((4 - (int)bar.Date.DayOfWeek + 7) % 7))
            .OrderBy(group => group.Key)
            .Select(group =>
            {
                var rows = group.OrderBy(x => x.Date).ToList();
                var low = rows.MinBy(x => x.Low)!;
                return new Bar(group.Key, rows[0].Open, rows.Max(x => x.High), rows.Min(x => x.Low), rows[^1].Close,
                    rows.Sum(x => x.Volume), rows.Any(x => x.Value.HasValue) ? rows.Sum(x => x.Value ?? 0m) : null, low.Date);
            })
            .Where(x => x.Date.AddDays(-1) <= today.Date)
            .ToList();
        return weekly;
    }

    private static double?[] CalculateRsi(IReadOnlyList<Bar> bars, int length)
    {
        var values = new double?[bars.Count];
        var averageGain = 0d;
        var averageLoss = 0d;
        var alpha = 1d / length;
        for (var i = 1; i < bars.Count; i++)
        {
            var delta = (double)(bars[i].Close - bars[i - 1].Close);
            var gain = Math.Max(delta, 0d);
            var loss = Math.Max(-delta, 0d);
            averageGain = i == 1 ? gain : alpha * gain + (1d - alpha) * averageGain;
            averageLoss = i == 1 ? loss : alpha * loss + (1d - alpha) * averageLoss;
            if (i < length) continue;
            values[i] = averageLoss == 0d ? (averageGain == 0d ? 50d : 100d) : 100d - 100d / (1d + averageGain / averageLoss);
        }
        return values;
    }

    private static List<int> FindPivotLows(IReadOnlyList<Bar> bars, int left, int right)
    {
        var pivots = new List<int>();
        for (var i = left; i < bars.Count - right; i++)
        {
            var isLow = true;
            for (var j = i - left; j < i; j++) if (bars[i].Low >= bars[j].Low) { isLow = false; break; }
            if (!isLow) continue;
            for (var j = i + 1; j <= i + right; j++) if (bars[i].Low > bars[j].Low) { isLow = false; break; }
            if (isLow) pivots.Add(i);
        }
        return pivots;
    }

    private static List<Divergence> FindDivergences(IReadOnlyList<Bar> bars, double?[] rsi, IReadOnlyList<int> pivots, Parameters p, double threshold)
    {
        var output = new List<Divergence>();
        for (var k = 0; k < pivots.Count; k++)
        {
            var second = pivots[k];
            if (rsi[second] is not { } secondRsi || secondRsi >= threshold) continue;
            for (var j = k - 1; j >= 0; j--)
            {
                var first = pivots[j];
                var gap = second - first;
                if (gap > p.MaximumGap) break;
                if (gap < p.MinimumGap || rsi[first] is not { } firstRsi || firstRsi >= threshold) continue;
                var interveningLow = bars.Skip(first + 1).Take(gap - 1).Select(x => x.Low).DefaultIfEmpty(decimal.MaxValue).Min();
                if (interveningLow < bars[second].Low) continue;
                if (bars[second].Low < bars[first].Low && secondRsi > firstRsi)
                {
                    var signal = second + p.Right;
                    if (signal < bars.Count) output.Add(new Divergence(first, second, signal));
                    break;
                }
            }
        }
        return output;
    }

    private static Confirmation FindConfirmation(IReadOnlyList<Bar> bars, Divergence signal, int window)
    {
        for (var i = signal.SignalIndex; i <= Math.Min(bars.Count - 1, signal.SignalIndex + window); i++)
        {
            if (bars[i].Close < bars[signal.SecondPivot].Low) return new(ConfirmationStatus.Invalid, -1);
            if (i > 0 && bars[i].Close > bars[i].Open && bars[i].Close > bars[i - 1].Close)
                return new(ConfirmationStatus.Confirmed, i);
        }
        return signal.SignalIndex + window <= bars.Count - 1
            ? new(ConfirmationStatus.Expired, -1)
            : new(ConfirmationStatus.Pending, -1);
    }

    private static IEnumerable<RsiDivergenceSuspect> FindDeveloping(
        IReadOnlyList<Bar> bars, double?[] rsi, IReadOnlyList<int> pivots, Parameters p, string symbol, string symbolKey)
    {
        for (var candidate = Math.Max(p.Left, bars.Count - p.Right); candidate < bars.Count; candidate++)
        {
            if (rsi[candidate] is not { } candidateRsi || candidateRsi >= SuspectRsiLevel) continue;
            if (bars[candidate].Low >= bars.Skip(candidate - p.Left).Take(p.Left).Min(x => x.Low)) continue;
            if (candidate + 1 < bars.Count && bars.Skip(candidate + 1).Any(x => x.Low < bars[candidate].Low)) continue;
            foreach (var first in pivots.Where(x => x < candidate).Reverse())
            {
                var gap = candidate - first;
                if (gap > p.MaximumGap) break;
                if (gap < p.MinimumGap || rsi[first] is not { } firstRsi || firstRsi >= SuspectRsiLevel) continue;
                if (bars.Skip(first + 1).Take(gap - 1).Any(x => x.Low < bars[candidate].Low)) continue;
                if (bars[candidate].Low >= bars[first].Low || candidateRsi <= firstRsi) continue;
                yield return new RsiDivergenceSuspect(symbol, symbolKey, p == Parameters.Daily ? "روزانه" : "هفتگی", "کف در حال شکل‌گیری",
                    bars[first].LowDate, bars[candidate].LowDate, bars[first].Low, bars[candidate].Low,
                    Round(firstRsi), Round(candidateRsi), Math.Max(0, p.Right - (bars.Count - 1 - candidate)));
                break;
            }
        }
    }

    private static IEnumerable<Trade> Backtest(IReadOnlyList<Bar> bars, IReadOnlyList<Divergence> divergences, Parameters p)
    {
        var nextFree = 0;
        foreach (var divergence in divergences)
        {
            var confirmation = FindConfirmation(bars, divergence, p.ConfirmWindow);
            if (confirmation.Status != ConfirmationStatus.Confirmed) continue;
            var entryIndex = confirmation.Index + 1;
            if (entryIndex >= bars.Count || entryIndex < nextFree) continue;
            var entry = bars[entryIndex].Open > 0 ? bars[entryIndex].Open : bars[entryIndex].Close;
            var stop = entry * (1m - StopLoss);
            var targetOne = entry * (1m + TargetOneRate);
            var targetTwo = entry * (1m + TargetTwoRate);
            var firstExit = (Index: -1, Price: 0m);
            var secondExit = (Index: -1, Price: 0m);
            var targetOneHit = false;
            var targetTwoHit = false;
            var reason = "time";
            var last = Math.Min(bars.Count - 1, entryIndex + p.MaximumHold - 1);
            for (var i = entryIndex; i <= last; i++)
            {
                if (bars[i].Low <= stop)
                {
                    var price = i == entryIndex ? stop : Math.Min(stop, bars[i].Open);
                    if (firstExit.Index < 0) firstExit = (i, price);
                    if (secondExit.Index < 0) secondExit = (i, price);
                    reason = "stop";
                    break;
                }
                if (firstExit.Index < 0 && bars[i].High >= targetOne) { firstExit = (i, targetOne); targetOneHit = true; }
                if (secondExit.Index < 0 && bars[i].High >= targetTwo) { secondExit = (i, targetTwo); targetTwoHit = true; }
                if (firstExit.Index >= 0 && secondExit.Index >= 0) { reason = "target-two"; break; }
            }
            if (firstExit.Index < 0) firstExit = (last, bars[last].Close);
            if (secondExit.Index < 0) secondExit = (last, bars[last].Close);
            var exitIndex = Math.Max(firstExit.Index, secondExit.Index);
            var returns = 0.5m * (firstExit.Price / entry - 1m) + 0.5m * (secondExit.Price / entry - 1m) - RoundTripCost;
            yield return new Trade(string.Empty, returns * 100m, exitIndex - entryIndex + 1, targetOneHit, targetTwoHit, reason);
            nextFree = exitIndex + 1;
        }
    }

    private static IReadOnlyList<RsiDivergenceBacktestSummary> Summarize(IReadOnlyList<Trade> trades)
        => trades.GroupBy(x => x.Timeframe).OrderBy(x => x.Key == "روزانه" ? 0 : 1).Select(group =>
        {
            var returns = group.Select(x => x.ReturnPercent).OrderBy(x => x).ToArray();
            var winners = returns.Where(x => x > 0).ToArray();
            var losers = returns.Where(x => x <= 0).ToArray();
            var lossTotal = Math.Abs(losers.Sum());
            var profitFactor = lossTotal == 0 ? 0 : winners.Sum() / lossTotal;
            return new RsiDivergenceBacktestSummary(group.Key, group.Count(),
                Percent(winners.Length, returns.Length), RoundDecimal(returns.Average()),
                RoundDecimal(returns.Length % 2 == 0 ? (returns[returns.Length / 2 - 1] + returns[returns.Length / 2]) / 2 : returns[returns.Length / 2]),
                RoundDecimal(profitFactor), Percent(group.Count(x => x.TargetOneHit), group.Count()),
                Percent(group.Count(x => x.TargetTwoHit), group.Count()), Percent(group.Count(x => x.Reason == "stop"), group.Count()),
                RoundDecimal((decimal)group.Average(x => x.BarsHeld)));
        }).ToArray();

    private static decimal Percent(int count, int total) => total == 0 ? 0 : RoundDecimal((decimal)count * 100m / total);
    private static double Round(double? value) => Math.Round(value ?? 0d, 2, MidpointRounding.ToEven);
    private static decimal RoundDecimal(decimal value) => decimal.Round(value, 2, MidpointRounding.ToEven);

    private sealed record Bar(DateTime Date, decimal Open, decimal High, decimal Low, decimal Close, decimal Volume, decimal? Value, DateTime? ExactLowDate = null)
    {
        public DateTime LowDate => ExactLowDate ?? Date;
    }
    private sealed record Divergence(int FirstPivot, int SecondPivot, int SignalIndex);
    private sealed record Confirmation(ConfirmationStatus Status, int Index);
    private sealed record Trade(string Timeframe, decimal ReturnPercent, int BarsHeld, bool TargetOneHit, bool TargetTwoHit, string Reason);
    private enum ConfirmationStatus { Pending, Confirmed, Invalid, Expired }
    private sealed record Parameters(int RsiLength, int Left, int Right, int ConfirmWindow, int MinimumGap, int MaximumGap, int RecentBars, int MinimumBars, int MaximumHold)
    {
        public static Parameters Daily { get; } = new(21, 3, 2, 5, 5, 60, 6, 120, 60);
        public static Parameters Weekly { get; } = new(12, 3, 2, 3, 4, 30, 4, 50, 26);
    }
}
