namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <summary>
    /// کنترل نرخ شروع درخواست‌ها بین کارگرهای موازی: حداقل فاصلهٔ ثابت بین شروع دو درخواست.
    /// اگر منبع سقف درخواست در دقیقه داشته باشد (<c>ProviderDescriptor.MaxRequestsPerMinute</c>) رعایت می‌شود.
    /// </summary>
    internal sealed class RequestPacer
    {
        private readonly TimeSpan _minGap;
        private readonly TimeProvider _time;
        private readonly object _sync = new();
        private DateTimeOffset _nextStart;

        public RequestPacer(int? maxRequestsPerMinute, TimeProvider time)
        {
            _time = time;
            _minGap = maxRequestsPerMinute is > 0 ? TimeSpan.FromMinutes(1.0 / maxRequestsPerMinute.Value) : TimeSpan.Zero;
            _nextStart = time.GetUtcNow();
        }

        /// <summary>تا نوبت مجاز شروع صبر می‌کند.</summary>
        public async Task WaitAsync(CancellationToken cancellationToken)
        {
            if (_minGap == TimeSpan.Zero)
                return;

            TimeSpan delay;
            lock (_sync)
            {
                var now = _time.GetUtcNow();
                var start = _nextStart > now ? _nextStart : now;
                _nextStart = start + _minGap;
                delay = start - now;
            }

            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, _time, cancellationToken);
        }
    }
}
