using MarketAnalyzer.MarketData.Application.Reading;

namespace MarketAnalyzer.MarketIntelligence.Application.Ports;

/// <summary>Adapts MarketData candle series to the analysis-owned candle value consumed by strategies.</summary>
public sealed class MarketDataAnalysisCandleReader : IAnalysisCandleReader
{
    private readonly IAdjustedCandleReader _reader;

    public MarketDataAnalysisCandleReader(IAdjustedCandleReader reader) => _reader = reader;

    public async Task<IReadOnlyList<AnalysisCandle>> GetCandlesAsync(
        MultiSymbolCandleQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var series = await _reader.GetSeriesAsync(query, cancellationToken);
        var marketOrder = query.Markets
            .Select((market, index) => new { Market = market, Index = index })
            .GroupBy(x => x.Market, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First().Index, StringComparer.OrdinalIgnoreCase);
        var results = new List<AnalysisCandle>();

        foreach (var item in series)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(item.TimeZoneId);
            foreach (var candle in item.Candles)
            {
                var localDate = TimeZoneInfo.ConvertTimeFromUtc(candle.OpenTimeUtc, timeZone);
                var timestamp = new DateTimeOffset(candle.OpenTimeUtc).ToUnixTimeMilliseconds();
                var timestampSeconds = new DateTimeOffset(candle.OpenTimeUtc).ToUnixTimeSeconds();
                results.Add(new AnalysisCandle
                {
                    Id = $"{timestampSeconds}_{item.Symbol.MarketKey}_{(int)item.Timeframe}_{item.Symbol.SymbolKey}",
                    InstrumentId = item.Symbol.SymbolKey,
                    MarketKey = item.Symbol.MarketKey,
                    Timeframe = item.Timeframe,
                    Open = candle.Open,
                    High = candle.High,
                    Low = candle.Low,
                    Close = candle.Close,
                    Volume = candle.Volume ?? 0m,
                    NetValue = candle.Value ?? 0m,
                    Date = localDate,
                    PersianDate = localDate.ToPersian(),
                    Timestamp = timestamp
                });
            }
        }

        return results
            .OrderBy(x => marketOrder.GetValueOrDefault(x.MarketKey, int.MaxValue))
            .ThenBy(x => x.InstrumentId, StringComparer.Ordinal)
            .ThenBy(x => x.Timeframe)
            .ThenBy(x => x.Date)
            .ToArray();
    }
}
