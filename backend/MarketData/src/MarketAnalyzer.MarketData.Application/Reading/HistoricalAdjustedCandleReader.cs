using MarketAnalyzer.MarketData.Application.Providers;
using MarketAnalyzer.MarketData.Application.Ingestion;
using MarketAnalyzer.MarketData.Domain;

namespace MarketAnalyzer.MarketData.Application.Reading
{
    /// <summary>خواندن سری تاریخی از مخزن جدید و اعمال ضرایب رویدادهای شرکتی برای منابع خام.</summary>
    public sealed class HistoricalAdjustedCandleReader : IAdjustedCandleReader
    {
        private readonly IMarketSymbolStore _symbols;
        private readonly IMarketDefinitionStore _markets;
        private readonly IHistoricalCandleStore _candles;
        private readonly ICorporateActionStore _actions;
        private readonly IMarketProviderResolver _providers;
        private readonly ICandleAggregator _aggregator;
        private readonly ITradingCalendarFactory _calendars;
        private readonly TimeProvider _time;

        public HistoricalAdjustedCandleReader(
            IMarketSymbolStore symbols,
            IMarketDefinitionStore markets,
            IHistoricalCandleStore candles,
            ICorporateActionStore actions,
            IMarketProviderResolver providers,
            ICandleAggregator aggregator,
            ITradingCalendarFactory calendars,
            TimeProvider time)
        {
            _symbols = symbols;
            _markets = markets;
            _candles = candles;
            _actions = actions;
            _providers = providers;
            _aggregator = aggregator;
            _calendars = calendars;
            _time = time;
        }

        public async Task<AdjustedCandleSeries?> GetSeriesAsync(CandleQuery query, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(query);
            if (query.SymbolId <= 0)
                throw new ArgumentOutOfRangeException(nameof(query), "SymbolId must be positive.");
            if (query.FromUtc.HasValue && query.ToUtc.HasValue && query.FromUtc.Value > query.ToUtc.Value)
                throw new ArgumentException("FromUtc must be earlier than or equal to ToUtc.", nameof(query));

            var symbol = await _symbols.GetByIdAsync(query.SymbolId, cancellationToken);
            if (symbol is null)
                return null;

            var markets = await _markets.GetAllAsync(cancellationToken);
            var market = markets.FirstOrDefault(x => x.Id == symbol.MarketId);
            if (market is null)
                return null;

            var provider = await _providers.ResolveAsync<IHistoricalCandleProvider>(
                market.Key,
                ProviderCapability.HistoricalCandles,
                cancellationToken);
            if (provider is null)
                throw new InvalidOperationException($"No active historical candle provider is configured for market '{market.Key}'.");

            var storageTimeframe = GetStorageTimeframe(query.Timeframe);
            var calendar = _calendars.Create(market.CalendarKind, market.TimeZoneId);
            var requestedFromUtc = query.FromUtc.HasValue ? AsUtc(query.FromUtc.Value) : (DateTime?)null;
            var requestedToUtc = query.ToUtc.HasValue ? AsUtc(query.ToUtc.Value) : (DateTime?)null;
            var fromUtc = ExpandFrom(requestedFromUtc, query.Timeframe, calendar);
            var toUtc = ExpandTo(requestedToUtc, query.Timeframe, calendar);
            var rows = await _candles.GetForSymbolsAsync(new[] { symbol.Id }, storageTimeframe, fromUtc, toUtc, cancellationToken);
            if (rows.Count == 0)
                return null;

            var symbolRows = rows.Where(x => x.SymbolId == symbol.Id).ToArray();
            if (symbolRows.Length == 0)
                return null;

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(market.TimeZoneId);
            IReadOnlyList<CorporateAction> actions = provider.Descriptor.Adjustment == AdjustmentMode.Raw
                ? await _actions.GetBySymbolAsync(symbol.Id, cancellationToken)
                : Array.Empty<CorporateAction>();

            var adjustedCandles = AdjustRows(symbolRows, actions, timeZone, symbol.SymbolKey);
            adjustedCandles = ApplyRequestedTimeframe(adjustedCandles, symbol.Id, query.Timeframe, calendar);
            if (requestedToUtc.HasValue)
                adjustedCandles = adjustedCandles.Where(x => x.OpenTimeUtc <= requestedToUtc.Value).ToArray();

            var currencyCode = symbol.CurrencyCode ?? market.CurrencyCode;
            return new AdjustedCandleSeries
            {
                Symbol = new SymbolRef
                {
                    SymbolId = symbol.Id,
                    MarketId = market.Id,
                    MarketKey = market.Key,
                    SymbolKey = symbol.SymbolKey,
                    CurrencyCode = currencyCode,
                    Name = symbol.Name,
                    PersianName = symbol.PersianName
                },
                Timeframe = query.Timeframe,
                CurrencyCode = currencyCode,
                CalendarKind = market.CalendarKind,
                TimeZoneId = market.TimeZoneId,
                SourceAdjustment = provider.Descriptor.Adjustment,
                Candles = adjustedCandles
            };
        }

        public async Task<IReadOnlyList<AdjustedCandleSeries>> GetSeriesAsync(MultiSymbolCandleQuery query, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(query);
            if (query.From.HasValue && query.To.HasValue && query.From.Value > query.To.Value)
                throw new ArgumentException("From must be earlier than or equal to To.", nameof(query));

            var selectedMarketKeys = query.Markets.Count > 0
                ? query.Markets.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
                : null;
            var selectedKeys = query.SymbolKeys.Count > 0
                ? new HashSet<string>(query.SymbolKeys.Where(x => !string.IsNullOrWhiteSpace(x)), StringComparer.OrdinalIgnoreCase)
                : null;
            var definitions = await _markets.GetAllAsync(cancellationToken);
            var result = new List<AdjustedCandleSeries>();

            var selectedMarkets = selectedMarketKeys is null
                ? definitions
                : definitions.Where(m => selectedMarketKeys.Contains(m.Key, StringComparer.OrdinalIgnoreCase)).ToArray();

            foreach (var market in selectedMarkets)
            {
                var symbols = await _symbols.GetByMarketAsync(market.Id, onlyActive: false, cancellationToken);
                if (selectedKeys is not null)
                    symbols = symbols.Where(x => selectedKeys.Contains(x.SymbolKey)).ToArray();
                if (symbols.Count == 0)
                    continue;

                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(market.TimeZoneId);
                var storageTimeframe = GetStorageTimeframe(query.Timeframe);
                var calendar = _calendars.Create(market.CalendarKind, market.TimeZoneId);
                var requestedFromUtc = query.From.HasValue ? ToUtcBound(query.From.Value, timeZone) : (DateTime?)null;
                var requestedToUtc = query.To.HasValue ? ToUtcBound(query.To.Value, timeZone) : (DateTime?)null;
                var fromUtc = ExpandFrom(requestedFromUtc, query.Timeframe, calendar);
                var toUtc = ExpandTo(requestedToUtc, query.Timeframe, calendar);
                var provider = await _providers.ResolveAsync<IHistoricalCandleProvider>(
                    market.Key,
                    ProviderCapability.HistoricalCandles,
                    cancellationToken);
                if (provider is null)
                    throw new InvalidOperationException($"No active historical candle provider is configured for market '{market.Key}'.");

                var rows = await _candles.GetForSymbolsAsync(
                    symbols.Select(x => x.Id).ToArray(), storageTimeframe, fromUtc, toUtc, cancellationToken);
                var rowsBySymbol = rows.GroupBy(x => x.SymbolId).ToDictionary(x => x.Key, x => x.ToArray());

                foreach (var symbol in symbols)
                {
                    if (!rowsBySymbol.TryGetValue(symbol.Id, out var symbolRows) || symbolRows.Length == 0)
                        continue;

                    IReadOnlyList<CorporateAction> actions = provider.Descriptor.Adjustment == AdjustmentMode.Raw
                        ? await _actions.GetBySymbolAsync(symbol.Id, cancellationToken)
                        : Array.Empty<CorporateAction>();

                    var adjustedCandles = AdjustRows(symbolRows, actions, timeZone, symbol.SymbolKey);
                    adjustedCandles = ApplyRequestedTimeframe(adjustedCandles, symbol.Id, query.Timeframe, calendar);
                    if (requestedToUtc.HasValue)
                        adjustedCandles = adjustedCandles.Where(x => x.OpenTimeUtc <= requestedToUtc.Value).ToArray();
                    if (adjustedCandles.Length == 0)
                        continue;

                    var currencyCode = symbol.CurrencyCode ?? market.CurrencyCode;
                    result.Add(new AdjustedCandleSeries
                    {
                        Symbol = new SymbolRef
                        {
                            SymbolId = symbol.Id,
                            MarketId = market.Id,
                            MarketKey = market.Key,
                            SymbolKey = symbol.SymbolKey,
                            CurrencyCode = currencyCode,
                            Name = symbol.Name,
                            PersianName = symbol.PersianName
                        },
                        Timeframe = query.Timeframe,
                        CurrencyCode = currencyCode,
                        CalendarKind = market.CalendarKind,
                        TimeZoneId = market.TimeZoneId,
                        SourceAdjustment = provider.Descriptor.Adjustment,
                        Candles = adjustedCandles
                    });
                }
            }

            return result;
        }

        private static DateTime AsUtc(DateTime value)
            => value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);

        private static Timeframe GetStorageTimeframe(Timeframe requested)
            => requested switch
            {
                Timeframe.Daily or Timeframe.Weekly or Timeframe.Mountly => Timeframe.Daily,
                _ => throw new NotSupportedException($"The {requested} timeframe is not enabled. MarketData stores daily candles only.")
            };

        private static DateTime ExpandFrom(DateTime? requestedFromUtc, Timeframe requested, ITradingCalendar calendar)
        {
            if (!requestedFromUtc.HasValue)
                return DateTime.MinValue;

            return requested is Timeframe.Weekly or Timeframe.Mountly
                ? calendar.AlignToPeriodStart(requestedFromUtc.Value, requested)
                : requestedFromUtc.Value;
        }

        private static DateTime? ExpandTo(DateTime? requestedToUtc, Timeframe requested, ITradingCalendar calendar)
        {
            if (!requestedToUtc.HasValue || requested is not (Timeframe.Weekly or Timeframe.Mountly))
                return requestedToUtc;

            var periodStart = calendar.AlignToPeriodStart(requestedToUtc.Value, requested);
            return calendar.GetPeriodEndUtc(periodStart, requested).AddTicks(-1);
        }

        private AdjustedCandle[] ApplyRequestedTimeframe(
            IReadOnlyList<AdjustedCandle> dailyCandles,
            int symbolId,
            Timeframe requested,
            ITradingCalendar calendar)
        {
            if (requested == Timeframe.Daily)
                return dailyCandles.ToArray();

            var baseRows = dailyCandles.Select(candle => new HistoricalCandle
            {
                SymbolId = symbolId,
                Timeframe = Timeframe.Daily,
                OpenTimeUtc = candle.OpenTimeUtc,
                Open = candle.Open,
                High = candle.High,
                Low = candle.Low,
                Close = candle.Close,
                Volume = candle.Volume,
                Value = candle.Value
            }).ToArray();

            return _aggregator.Aggregate(baseRows, requested, calendar, _time.GetUtcNow().UtcDateTime)
                .Select(candle => new AdjustedCandle
                {
                    OpenTimeUtc = AsUtc(candle.OpenTimeUtc),
                    Open = candle.Open,
                    High = candle.High,
                    Low = candle.Low,
                    Close = candle.Close,
                    Volume = candle.Volume,
                    Value = candle.Value
                })
                .ToArray();
        }

        private static AdjustedCandle[] AdjustRows(
            IReadOnlyList<HistoricalCandle> rows,
            IReadOnlyList<CorporateAction> actions,
            TimeZoneInfo timeZone,
            string symbolKey)
            => rows.Select(row =>
            {
                var utc = AsUtc(row.OpenTimeUtc);
                var localDate = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(utc, timeZone));
                var factor = actions
                    .Where(action => localDate < action.EffectiveDate)
                    .Aggregate(1m, (current, action) => current * action.AdjustmentFactor);
                if (factor <= 0m)
                    throw new InvalidOperationException($"Invalid adjustment factor for symbol '{symbolKey}' at {localDate:yyyy-MM-dd}.");

                return new AdjustedCandle
                {
                    OpenTimeUtc = utc,
                    Open = row.Open * factor,
                    High = row.High * factor,
                    Low = row.Low * factor,
                    Close = row.Close * factor,
                    Volume = row.Volume,
                    Value = row.Value
                };
            }).ToArray();

        private static DateTime ToUtcBound(DateTime value, TimeZoneInfo timeZone)
        {
            if (value.Kind == DateTimeKind.Utc)
                return value;

            return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(value, DateTimeKind.Unspecified), timeZone);
        }
    }
}
