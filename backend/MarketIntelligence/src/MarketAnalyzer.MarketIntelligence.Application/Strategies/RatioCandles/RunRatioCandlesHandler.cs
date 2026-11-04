using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketIntelligence.Application.Ports;
using MarketAnalyzer.MarketIntelligence.Domain;
using MarketAnalyzer.SharedKernel;

namespace MarketAnalyzer.MarketIntelligence.Application.Strategies.RatioCandles;

public sealed class RunRatioCandlesHandler
{
    private static readonly string[] SupportedMarkets = ["tse", "macro", "crypto", "tse-index", "crypto-toman"];
    private readonly IAnalysisCandleReader _candles;

    public RunRatioCandlesHandler(IAnalysisCandleReader candles) => _candles = candles;

    public async Task<IReadOnlyList<CandleSeriesResult>> HandleAsync(
        RunRatioCandlesQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Symbols.Count == 0)
            throw new ArgumentException("At least one symbol is required.", nameof(request));
        if (request.StartDate > request.EndDate)
            throw new ArgumentException("Start date must be earlier than or equal to end date.", nameof(request));

        var symbols = request.Symbols.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var candles = await _candles.GetCandlesAsync(new MultiSymbolCandleQuery
        {
            From = request.StartDate,
            To = request.EndDate,
            Timeframe = Timeframe.Daily,
            SymbolKeys = symbols,
            Markets = SupportedMarkets
        }, cancellationToken);

        var allDates = candles
            .GroupBy(candle => candle.PersianDate)
            .Select(group => new { PersianDate = group.Key, Date = group.Max(candle => candle.Date) })
            .OrderBy(day => day.Date)
            .ToList();

        var results = new List<CandleSeriesResult>(symbols.Length);
        foreach (var symbol in symbols)
        {
            var symbolCandles = candles.Where(candle => candle.InstrumentId == symbol).ToArray();
            var firstClose = symbolCandles.OrderBy(candle => candle.Date).FirstOrDefault()?.Close ?? 1m;
            var dailyByPersianDate = symbolCandles
                .GroupBy(candle => candle.PersianDate)
                .ToDictionary(group => group.Key, group => group.OrderByDescending(candle => candle.Date).First());

            decimal? lastGrowth = null;
            var points = new List<CandleResult>(allDates.Count);
            foreach (var day in allDates)
            {
                if (dailyByPersianDate.TryGetValue(day.PersianDate, out var candle))
                {
                    var growth = candle.Close / firstClose;
                    points.Add(new CandleResult
                    {
                        Date = candle.Date.ToDateOnly(),
                        PersianDate = candle.PersianDate,
                        Growth = growth
                    });
                    lastGrowth = growth;
                }
                else
                {
                    points.Add(new CandleResult
                    {
                        Date = day.Date.ToDateOnly(),
                        PersianDate = day.PersianDate,
                        Growth = lastGrowth ?? 0m
                    });
                }
            }

            results.Add(new CandleSeriesResult
            {
                InstrumentId = symbol,
                Symbol = symbol,
                Results = points
            });
        }

        return results;
    }
}
