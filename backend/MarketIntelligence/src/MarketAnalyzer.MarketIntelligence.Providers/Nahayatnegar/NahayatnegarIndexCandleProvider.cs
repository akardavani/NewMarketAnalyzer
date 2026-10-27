using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>کندل روزانهٔ شاخص‌های بورس از نهایت‌نگر. شاخص تعدیل ندارد (<c>NotApplicable</c>).</summary>
    public sealed class NahayatnegarIndexCandleProvider : IHistoricalCandleProvider
    {
        private readonly NahayatnegarCandleClient _client;

        /// <summary>سازنده.</summary>
        public NahayatnegarIndexCandleProvider(NahayatnegarCandleClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public ProviderDescriptor Descriptor { get; } = Markets.TseIndex(NahayatnegarCandleClient.ProviderKey);

        /// <inheritdoc />
        public Task<FetchResult<CandleBatch>> FetchCandlesAsync(CandleRequest request, CancellationToken cancellationToken)
            => _client.FetchAsync(request, cancellationToken);
    }
}
