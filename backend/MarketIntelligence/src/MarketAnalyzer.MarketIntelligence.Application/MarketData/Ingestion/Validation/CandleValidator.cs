using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>پیاده‌سازی <see cref="ICandleValidator"/>؛ بدون وضعیت و thread-safe.</summary>
    public sealed class CandleValidator : ICandleValidator
    {
        private const int MaxIssues = 20;

        /// <inheritdoc />
        public ValidatedCandles Validate(CandleBatch batch, ITradingCalendar calendar, DateTime nowUtc)
        {
            var issues = new List<string>();
            var byTime = new Dictionary<DateTime, CandleData>();
            var rejected = 0;
            var repaired = 0;
            var futureLimit = nowUtc.AddDays(1);

            foreach (var raw in batch.Candles)
            {
                var openUtc = calendar.AlignToPeriodStart(ToUtc(raw.OpenTimeUtc), batch.Timeframe);

                if (raw.Open <= 0 || raw.High <= 0 || raw.Low <= 0 || raw.Close <= 0)
                {
                    rejected++;
                    AddIssue(issues, $"{batch.SymbolKey} {openUtc:O}: non-positive price");
                    continue;
                }

                if (raw.Volume < 0 || raw.Value < 0)
                {
                    rejected++;
                    AddIssue(issues, $"{batch.SymbolKey} {openUtc:O}: negative volume/value");
                    continue;
                }

                if (openUtc > futureLimit)
                {
                    rejected++;
                    AddIssue(issues, $"{batch.SymbolKey} {openUtc:O}: future-dated candle");
                    continue;
                }

                // Problem: منبع‌ها گاهی قیمت‌های گردشده می‌دهند و High کمتر از Open/Close یا Low بیشتر از آن‌ها می‌شود.
                // Fix: بازه را به اندازهٔ لازم باز می‌کنیم تا شرط Low ≤ Open,Close ≤ High برقرار شود (کندل دور ریخته نمی‌شود).
                var high = Math.Max(raw.High, Math.Max(raw.Open, raw.Close));
                var low = Math.Min(raw.Low, Math.Min(raw.Open, raw.Close));
                if (high != raw.High || low != raw.Low)
                {
                    repaired++;
                    AddIssue(issues, $"{batch.SymbolKey} {openUtc:O}: High/Low repaired");
                }

                // کندل تکراری: آخری برنده است (منبع ممکن است همان بازه را اصلاح کرده باشد).
                byTime[openUtc] = new CandleData
                {
                    OpenTimeUtc = openUtc,
                    Open = raw.Open,
                    High = high,
                    Low = low,
                    Close = raw.Close,
                    Volume = raw.Volume,
                    Value = raw.Value
                };
            }

            var ordered = byTime.Values.OrderBy(x => x.OpenTimeUtc).ToList();
            var closed = new List<CandleData>(ordered.Count);
            CandleData? open = null;

            foreach (var candle in ordered)
            {
                if (calendar.IsPeriodClosed(candle.OpenTimeUtc, batch.Timeframe, nowUtc))
                    closed.Add(candle);
                else
                    open = candle; // آخرین کندل ناتمام (به ترتیب صعودی، جدیدترین می‌ماند)
            }

            return new ValidatedCandles
            {
                Closed = closed,
                Open = open,
                RejectedCount = rejected,
                RepairedCount = repaired,
                Issues = issues
            };
        }

        private static DateTime ToUtc(DateTime value) => value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

        private static void AddIssue(List<string> issues, string message)
        {
            if (issues.Count < MaxIssues)
                issues.Add(message);
        }
    }
}
