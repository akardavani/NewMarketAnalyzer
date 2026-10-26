using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Options;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Ingestion
{
    /// <summary>
    /// پیاده‌سازی <see cref="IAdjustmentDriftDetector"/>.
    /// الگوریتم: کندل‌های مشترک را با OpenTimeUtc جفت می‌کنیم و نسبت Close جدید به قدیم را می‌گیریم. اگر قدیمی‌ترین
    /// <see cref="IngestionOptions.DriftSampleSize"/> جفت همه از تلرانس بیشتر فرق داشته باشند و نسبت‌هایشان تقریباً برابر باشد،
    /// ضریب ثابت اعلام می‌شود. تعدیل همیشه کندل‌های <b>قدیمی</b> را عوض می‌کند، پس نمونهٔ قدیمی‌ترین‌ها معیار درستی است
    /// و نویسهٔ گردکردن یک کندل را به‌عنوان drift نمی‌گیرد.
    /// </summary>
    public sealed class AdjustmentDriftDetector : IAdjustmentDriftDetector
    {
        private readonly IngestionOptions _options;

        /// <summary>سازنده.</summary>
        public AdjustmentDriftDetector(IOptions<IngestionOptions> options)
        {
            _options = options.Value;
        }

        /// <inheritdoc />
        public DriftAssessment Detect(IReadOnlyList<HistoricalCandle> stored, IReadOnlyList<CandleData> incomingClosed)
        {
            var incomingByTime = new Dictionary<DateTime, decimal>();
            foreach (var c in incomingClosed)
                incomingByTime[c.OpenTimeUtc] = c.Close;

            var pairs = new List<(DateTime Time, decimal Ratio)>();
            foreach (var s in stored.OrderBy(x => x.OpenTimeUtc))
            {
                if (s.Close > 0 && incomingByTime.TryGetValue(s.OpenTimeUtc, out var incoming) && incoming > 0)
                    pairs.Add((s.OpenTimeUtc, incoming / s.Close));
            }

            if (pairs.Count < _options.MinComparedCandles)
                return DriftAssessment.None(pairs.Count);

            bool IsMismatch((DateTime Time, decimal Ratio) p) => Math.Abs(p.Ratio - 1m) > _options.PriceRelativeTolerance;

            var mismatched = pairs.Count(IsMismatch);
            if (mismatched == 0)
                return DriftAssessment.None(pairs.Count);

            var sample = pairs.Take(Math.Min(_options.DriftSampleSize, pairs.Count)).ToList();
            if (sample.All(IsMismatch))
            {
                var ratios = sample.Select(x => x.Ratio).OrderBy(x => x).ToList();
                var median = ratios[ratios.Count / 2];
                var spread = (ratios[^1] - ratios[0]) / median;

                if (spread <= _options.RatioConsistencyTolerance)
                {
                    var boundaryIndex = pairs.FindIndex(p => !IsMismatch(p));
                    var hasBoundary = boundaryIndex >= 0;

                    return new DriftAssessment
                    {
                        Kind = DriftKind.ConstantRatio,
                        Ratio = median,
                        EffectiveOpenTimeUtc = hasBoundary ? pairs[boundaryIndex].Time : pairs[^1].Time,
                        IsEffectiveTimeApproximate = !hasBoundary,
                        ComparedCount = pairs.Count,
                        MismatchedCount = mismatched
                    };
                }
            }

            return new DriftAssessment
            {
                Kind = DriftKind.Inconsistent,
                ComparedCount = pairs.Count,
                MismatchedCount = mismatched
            };
        }
    }
}
